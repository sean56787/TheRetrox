using System;
using System.Collections;
using System.Collections.Generic;
using UniGLTF;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Task = System.Threading.Tasks.Task;

public class NPCController : MonoBehaviour
{
    public NPCAnimator npcAnimator;
    public Transform throwFrom;
    public Transform productAreaToThrow;
    public Transform productInitArea;
    public Transform moneyInitArea;
    public Transform moneyAreaToThrow;
    public Transform targetCabinetTransform;
    public List<GameObject> unCheckedProductsList = new();
    public GameObject moneyObj;
    public WaypointNavigator waypointNavigator;
    public Transform unCheckedProducts;
    public Transform checkedProducts;
    public int currentQueueIndex = -1;
    public bool setShoppingRouteFlag;
    public bool enterStoreFlag;
    public bool exitStoreFlag;
    public bool takingOutProductFlag;
    
    private Transform _targetCounterQueueTransform;
    public NPCCustomer npcCustomer;
    private Product _targetProduct;
    private ProductCabinet _targetCabinet;
    private Queue<Product> _targetProductQueue = new();
    private Queue<Product> _currentProductQueue = new();
    private NPCShoppingList _npcShoppingList;
    private bool _isPaid;
    private bool _receivedChanges;
    private bool _isPlayingTriggerAnimation;
    private bool _isRaging;
    public enum NPCState
    {
        IsWanderingAround,
        EnterStore,
        SetShoppingRoute,
        FindNextCabinet,
        MovingToCabinet,
        TakeProduct,
        JoinCounterQueue,
        SetCounterQueue,
        MovingToNextCounterQueue,
        CheckQueuePosition,
        Checkout,
        WaitingForCheckout,
        Pay,
        WaitingForReceiveChange,
        ExitStore,
        GoBackToStreet,
    }
    [Header("NPC State")]
    public NPCState currentNpcState;
    private void Awake()
    {
        _npcShoppingList = GetComponent<NPCShoppingList>();
        npcCustomer = GetComponent<NPCCustomer>();
        npcAnimator = GetComponent<NPCAnimator>();
        waypointNavigator = GetComponent<WaypointNavigator>();
        
        ChangeState(NPCState.IsWanderingAround);
    }

    private void Update()
    {
        switch (currentNpcState)
        {
            case NPCState.IsWanderingAround:
                waypointNavigator.isInStore = false;
                break;
            case NPCState.EnterStore:
                waypointNavigator.isInStore = true;
                StartCoroutine(EnterStore());
                break;
            case NPCState.SetShoppingRoute:
                SetShoppingRoute();
                break;
            case NPCState.FindNextCabinet:
                MoveToNextCabinet();
                break;
            case NPCState.MovingToCabinet:
                StartCoroutine(MovingToCabinet());
                break;
            case NPCState.TakeProduct:
                if(!_isPlayingTriggerAnimation) StartCoroutine(RequestProduct());
                break;
            case NPCState.JoinCounterQueue:
                JoinCounterQueue();
                break;
            case NPCState.SetCounterQueue:
                SetQueuePosition(CounterQueueManager.instance.customerQueuePositions[currentQueueIndex]);
                ChangeState(NPCState.MovingToNextCounterQueue);
                break;
            case NPCState.MovingToNextCounterQueue:
                StartCoroutine(MovingToNextCounterQueue());
                break;
            case NPCState.CheckQueuePosition:
                CheckQueuePosition();
                break;
            case NPCState.Checkout:
                if(!takingOutProductFlag && _currentProductQueue.Count > 0) StartCoroutine(TakeoutProduct());
                break;
            case NPCState.WaitingForCheckout:
                StartCoroutine(WaitingForCheckout());
                break;
            case NPCState.Pay:
                if (!_isPaid) StartCoroutine(Pay());
                break;
            case NPCState.WaitingForReceiveChange:
                WaitingForReceiveChange();
                break;
            case NPCState.ExitStore:
                StartCoroutine(ExitStore());
                break;
            case NPCState.GoBackToStreet:
                break;
        }
    }
    IEnumerator EnterStore()
    {
        if (!enterStoreFlag)
        {
            waypointNavigator.SetAgentInStoreDestination(waypointNavigator.entryPoint);
            enterStoreFlag = true;
            while (!waypointNavigator.isAgentStopped && 
                   (waypointNavigator.npcNavAgent.pathPending || 
                    waypointNavigator.npcNavAgent.remainingDistance > waypointNavigator.npcNavAgent.stoppingDistance))
            {
                if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.InHurry) npcAnimator.NPC_RunAnimation();
                else npcAnimator.NPC_WalkAnimation();
                yield return null;
            }
            ChangeState(NPCState.SetShoppingRoute);
            enterStoreFlag = false;
        }
    }
    void SetShoppingRoute()
    {
        if (!setShoppingRouteFlag)
        {
            setShoppingRouteFlag = true;
            foreach (var targetProduct in _npcShoppingList.targetShoppingList)
            {
                _targetProductQueue.Enqueue(targetProduct);
            }
            ChangeState(NPCState.FindNextCabinet);
        }
    }
    void MoveToNextCabinet()
    {
        if (_targetProductQueue.Count == 0)
        {
            ChangeState(NPCState.JoinCounterQueue);
            return;
        }
        _targetProduct = _targetProductQueue.Dequeue();
        _targetCabinet = CabinetManager.instance.GetCabinet(_targetProduct.category);
        if (_targetCabinet.cabinetCategory == _targetProduct.category)
        {
            targetCabinetTransform = _targetCabinet.GetValidPosition();
            if (targetCabinetTransform != null)
            {
                waypointNavigator.SetAgentInStoreDestination(targetCabinetTransform);
                ChangeState(NPCState.MovingToCabinet);
            }
            else
            {
                Debug.LogError($"{_targetCabinet.name} valid position not found");
            }
        }
    }
    private IEnumerator MovingToCabinet()
    {
        //pathPending => 還在規劃路線
        while (!waypointNavigator.isAgentStopped &&
            (waypointNavigator.npcNavAgent.pathPending || 
             waypointNavigator.npcNavAgent.remainingDistance > waypointNavigator.npcNavAgent.stoppingDistance))
        {
            if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.InHurry) npcAnimator.NPC_RunAnimation();
                else npcAnimator.NPC_WalkAnimation();
            yield return null; //等下一偵
        }
        ChangeState(NPCState.TakeProduct);
    }
    IEnumerator RequestProduct()
    {
        _isPlayingTriggerAnimation = true;
        npcAnimator.NPC_GrabAnimation();
        yield return new WaitForSeconds(2.333f);
        
        _targetCabinet.CustomerProductRequestInvoker(_currentProductQueue, _targetProduct);
        if (_targetProductQueue.Count == 0)
        {
            _targetCabinet.ReleaseValidPosition(targetCabinetTransform);
            ChangeState(NPCState.JoinCounterQueue);
        }
        else
        {
            _targetCabinet.ReleaseValidPosition(targetCabinetTransform);
            ChangeState(NPCState.FindNextCabinet);
        }
        
        _isPlayingTriggerAnimation = false;
    }
    void JoinCounterQueue()
    {
        CounterQueueManager.instance.JoinQueue(this);
        ChangeState(NPCState.SetCounterQueue);
    }
    public void SetQueuePosition(Transform queuePosition)
    {
        waypointNavigator.SetAgentInStoreDestination(queuePosition);
    }
    public IEnumerator MovingToNextCounterQueue()
    {
        while (!waypointNavigator.isAgentStopped &&
               (waypointNavigator.npcNavAgent.pathPending || waypointNavigator.npcNavAgent.remainingDistance > waypointNavigator.npcNavAgent.stoppingDistance))
        {
            if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.InHurry) npcAnimator.NPC_RunAnimation();
                else npcAnimator.NPC_WalkAnimation();
            yield return null; //等下一偵
        }
        ChangeState(NPCState.CheckQueuePosition);
    }
    private void CheckQueuePosition()
    {
        npcAnimator.ResetAnimation();
        if(currentQueueIndex == 0)
        {
            ChangeState(NPCState.Checkout);
        }
    }

    private IEnumerator TakeoutProduct()
    {
        takingOutProductFlag = true;
        AudioSource tempAS = SoundManager.instance.GetAvailableAudioSource();
        if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.Sloth)
        {
            npcAnimator.NPC_PutdownSlothAnimation();
        }
        yield return new WaitForSeconds(0.5f);
        while (_currentProductQueue.Count > 0)
        {
            Product product = _currentProductQueue.Dequeue();
            for (int i = 0; i < product.quantity; i++)
            {
                GameObject unCheckedProduct;
                if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.InHurry)
                {
                    Debug.Log("Throw");
                    SoundManager.instance.PlayClip_Angry();
                    npcAnimator.NPC_ThrowAnimation();
                    yield return new WaitForSeconds(0.65f);
                    unCheckedProduct = ProductManager.instance.InstantiateProductObj(product, throwFrom);
                    Rigidbody rb = unCheckedProduct.GetComponent<Rigidbody>();
                    Vector3 direction = (productAreaToThrow.position - transform.position).normalized;
                    rb.AddForce(direction * npcCustomer.throwStrength, ForceMode.Impulse);
                }
                else
                {
                    if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.Sloth)
                    {
                        Debug.Log("sloth");
                        yield return new WaitForSeconds(1.5f);
                        if(!tempAS.isPlaying) SoundManager.instance.PlayClip_Snore(tempAS);
                    }
                    else
                    {
                        Debug.Log("normal");
                        npcAnimator.NPC_PutdownNormalAnimation();
                        yield return new WaitForSeconds(0.65f);
                    }
                    SoundManager.instance.PlayClip_ItemPop();
                    unCheckedProduct = ProductManager.instance.InstantiateProductObj(product, productInitArea);
                }
                unCheckedProductsList.Add(unCheckedProduct);
                unCheckedProduct.transform.SetParent(unCheckedProducts);
                yield return new WaitForSeconds(1f);
            } 
        }
        if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.Sloth) CancelInvoke(nameof(SoundManager.instance.PlayClip_Snore));
        npcAnimator.ResetAnimation();
        Debug.Log("WaitingForCheckout");
        ChangeState(NPCState.WaitingForCheckout);
    }
    
    public void DestroyCheckedProducts() // TODO:商品回到顧客身上
    {
        foreach (var p in unCheckedProductsList)
        {
            Destroy(p);
        }
    }
    
    IEnumerator WaitingForCheckout()
    {
        while (!IsAllCheckedOut())
        {
            if (!_isRaging && npcCustomer.npcPersonality.personality == NPCPersonality.Personality.InHurry) StartCoroutine(Raging());
            yield return null;
        }
        ChangeState(NPCState.Pay);
        
        bool IsAllCheckedOut()
        {
            foreach (var p in unCheckedProductsList)
            {
                if (!p.GetComponent<ProductObj>().isChecked)
                {
                    return false;
                }
                p.transform.SetParent(checkedProducts);
            }
            return true;
        }
    }
        
    IEnumerator Pay()
    {
        _isPaid = true;
        if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.InHurry)
        {
            SoundManager.instance.PlayClip_Angry();
            npcAnimator.NPC_ThrowAnimation();
            yield return new WaitForSeconds(0.3f);
            moneyObj = Instantiate(npcCustomer.moneyPrefab, throwFrom.position, Quaternion.LookRotation(transform.up));
            Rigidbody rb = moneyObj.GetComponent<Rigidbody>();
            Vector3 throwDirection = (moneyAreaToThrow.position - npcCustomer.throwPos.position).normalized;
            rb.AddForce(throwDirection * npcCustomer.throwStrength, ForceMode.Impulse);
        }
        else
        {
            npcAnimator.NPC_PutdownNormalAnimation();
            SoundManager.instance.PlayClip_ItemPop();
            moneyObj = Instantiate(npcCustomer.moneyPrefab, moneyInitArea.position, Quaternion.LookRotation(transform.up));
        }
        moneyObj.GetComponent<MoneyObj>().money = _npcShoppingList.GetCustomerPaid();
        moneyObj.GetComponent<MoneyObj>().playerReceived = false;
        ChangeState(NPCState.WaitingForReceiveChange);
    }
    
    void WaitingForReceiveChange()
    {
        Debug.Log("WaitingForReceiveChange");
        
        if(moneyObj.GetComponent<MoneyObj>().playerReceived)
        {
            if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.InHurry) SoundManager.instance.PlayClip_WasteMyTime();
            CounterQueueManager.instance.RemoveFirst();
            CounterQueueManager.instance.UpdateQueue();
            ChangeState(NPCState.ExitStore);
            DestroyCheckedProducts();
            unCheckedProductsList.Clear();
        }
    }
    
    IEnumerator ExitStore() 
    {
        if (!exitStoreFlag)
        {
            exitStoreFlag = true;
            Debug.Log($"ExitStore");
            waypointNavigator.SetAgentInStoreDestination(waypointNavigator.exitPoint);
            while (!waypointNavigator.isAgentStopped &&
                   (waypointNavigator.npcNavAgent.pathPending ||
                    waypointNavigator.npcNavAgent.remainingDistance > waypointNavigator.npcNavAgent.stoppingDistance))
            {
                if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.InHurry) npcAnimator.NPC_RunAnimation();
                else npcAnimator.NPC_WalkAnimation();
                yield return null; //等下一偵
            }
            waypointNavigator.isAlreadyBought = true;
            ChangeState(NPCState.IsWanderingAround);
        }
    }
    public void ChangeState(NPCState newNpcState)
    {
        currentNpcState = newNpcState;
    }

    IEnumerator Raging()
    {
        _isRaging = true;
        SoundManager.instance.PlayClip_HurryUp();
        int randomRageTime = Random.Range(5, 10);
        yield return new WaitForSeconds(randomRageTime);
        _isRaging = false;
    } 
}
