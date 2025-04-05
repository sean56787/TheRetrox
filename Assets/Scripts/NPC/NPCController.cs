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
    
    public enum NPCState // NPC 狀態機
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
    public NPCState currentNpcState; // 目前NPC狀態
    
    private void Awake()
    {
        _npcShoppingList = GetComponent<NPCShoppingList>();
        npcCustomer = GetComponent<NPCCustomer>();
        npcAnimator = GetComponent<NPCAnimator>();
        waypointNavigator = GetComponent<WaypointNavigator>();
        
        ChangeState(NPCState.IsWanderingAround); //預設 NPC "閒晃"
    }

    private void Update()
    {
        CheckDaytime(); // 白天才有機率進入商店
        StateTree(); //NPC狀態機循環
    }
    
    IEnumerator EnterStore() // 進入商店
    {
        if (!enterStoreFlag)
        {
            waypointNavigator.SetAgentInStoreDestination(waypointNavigator.entryPoint); // agent導引NPC到達商店位置
            enterStoreFlag = true;
            while (!waypointNavigator.isAgentStopped &&  // 如果NPC還沒停
                   (waypointNavigator.npcNavAgent.pathPending || // 如果NPC還在規劃路線
                    waypointNavigator.npcNavAgent.remainingDistance > waypointNavigator.npcNavAgent.stoppingDistance)) // 如果NPC還沒到達
            {
                if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.InHurry) npcAnimator.NPC_RunAnimation(); // 暴怒NPC要用快跑的
                else npcAnimator.NPC_WalkAnimation(); // 普通NPC用走的
                yield return null;
            }
            ChangeState(NPCState.SetShoppingRoute);
            enterStoreFlag = false;
        }
    }
    
    void SetShoppingRoute() // NPC開始規劃要買啥
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
    
    void MoveToNextCabinet() // 準備走向下一個商品櫃
    {
        if (_targetProductQueue.Count == 0)
        {
            ChangeState(NPCState.JoinCounterQueue);
            return;
        }
        _targetProduct = _targetProductQueue.Dequeue(); // 現在要拿的商品(從queue退出來)
        _targetCabinet = CabinetManager.instance.GetCabinet(_targetProduct.category); // 走向下一個商品櫃
        if (_targetCabinet.cabinetCategory == _targetProduct.category)
        {
            targetCabinetTransform = _targetCabinet.GetValidPosition(); // 獲取商品櫃可以站的位置
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
    
    private IEnumerator MovingToCabinet() // 正在走向目標商品櫃
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
    
    IEnumerator RequestProduct() // 拿商品
    {
        _isPlayingTriggerAnimation = true;
        npcAnimator.NPC_GrabAnimation(); // 拿商品動畫
        yield return new WaitForSeconds(2.333f);
        
        _targetCabinet.CustomerProductRequestInvoker(_currentProductQueue, _targetProduct); // 跟商品櫃互動
        if (_targetProductQueue.Count == 0) // 如果已經都拿好了，就去結帳
        {
            _targetCabinet.ReleaseValidPosition(targetCabinetTransform);
            ChangeState(NPCState.JoinCounterQueue);
        }
        else // 還有要拿的商品，回到FindNextCabinet
        {
            _targetCabinet.ReleaseValidPosition(targetCabinetTransform);
            ChangeState(NPCState.FindNextCabinet);
        }
        
        _isPlayingTriggerAnimation = false;
    }
    
    void JoinCounterQueue()
    {
        CounterQueueManager.instance.JoinQueue(this); // 被加進排隊Queue
        ChangeState(NPCState.SetCounterQueue);
    }
    
    public void SetQueuePosition(Transform queuePosition)
    {
        waypointNavigator.SetAgentInStoreDestination(queuePosition);
    }
    
    public IEnumerator MovingToNextCounterQueue() // 移動到下一個排隊
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
    
    private void CheckQueuePosition() // 檢查是否為第一個結帳的人，是就開始拿出商品
    {
        npcAnimator.ResetAnimation();
        if(currentQueueIndex == 0)
        {
            ChangeState(NPCState.Checkout);
        }
    }

    private IEnumerator TakeoutProduct() // 開始結帳 拿出商品
    {
        takingOutProductFlag = true;
        AudioSource tempAS = SoundManager.instance.GetAvailableAudioSource();
        if(tempAS == null) Debug.LogError("Audio Source is null");
        if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.Sloth) // 如果是懶人 放商品要很慢
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
                if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.InHurry) // 如果是暴怒的人 商品要用丟的
                {
                    StartCoroutine(SoundManager.instance.PlayClip_Angry());
                    npcAnimator.NPC_ThrowAnimation();
                    yield return new WaitForSeconds(0.65f);
                    unCheckedProduct = ProductManager.instance.InstantiateProductObj(product, throwFrom);
                    Rigidbody rb = unCheckedProduct.GetComponent<Rigidbody>();
                    Vector3 direction = (productAreaToThrow.position - transform.position).normalized;
                    rb.AddForce(direction * npcCustomer.throwStrength, ForceMode.Impulse);
                }
                else
                {
                    if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.Sloth) // 如果是懶人 加上打呼聲
                    {
                        yield return new WaitForSeconds(1.5f);
                        if(!tempAS.isPlaying) StartCoroutine(SoundManager.instance.PlayClip_Snore(tempAS));
                    }
                    else
                    {
                        npcAnimator.NPC_PutdownNormalAnimation();
                        yield return new WaitForSeconds(0.65f);
                    }
                    StartCoroutine(SoundManager.instance.PlayClip_ItemPop());
                    unCheckedProduct = ProductManager.instance.InstantiateProductObj(product, productInitArea); // 正常商品生成
                }
                unCheckedProductsList.Add(unCheckedProduct);
                // unCheckedProduct.transform.SetParent(unCheckedProducts);
                if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.InHurry) yield return new WaitForSeconds(0.1f);
                else yield return new WaitForSeconds(1f);
            } 
        }
        // if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.Sloth) CancelInvoke(nameof(SoundManager.instance.PlayClip_Snore));
        npcAnimator.ResetAnimation();
        ChangeState(NPCState.WaitingForCheckout);
    }
    
    public void DestroyCheckedProducts() // 顧客離開要刪除商品
    {
        foreach (var p in unCheckedProductsList)
        {
            Destroy(p);
        }
    }
    
    IEnumerator WaitingForCheckout() //等待玩家結帳
    {
        while (!IsAllCheckedOut())
        {
            if (!_isRaging && npcCustomer.npcPersonality.personality == NPCPersonality.Personality.InHurry) StartCoroutine(Raging()); // 暴怒的人 會催人
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
                // p.transform.SetParent(checkedProducts);
            }
            return true;
        }
    }
        
    IEnumerator Pay() // 顧客付錢
    {
        _isPaid = true;
        if (npcCustomer.npcPersonality.personality == NPCPersonality.Personality.InHurry) //暴怒的人用丟的
        {
            StartCoroutine(SoundManager.instance.PlayClip_Angry());
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
            StartCoroutine(SoundManager.instance.PlayClip_ItemPop());
            moneyObj = Instantiate(npcCustomer.moneyPrefab, moneyInitArea.position, Quaternion.LookRotation(transform.up));
        }
        moneyObj.GetComponent<MoneyObj>().money = _npcShoppingList.GetCustomerPaid();
        moneyObj.GetComponent<MoneyObj>().playerReceived = false;
        moneyObj.GetComponent<MoneyObj>().Highlight();
        ChangeState(NPCState.WaitingForReceiveChange);
    }
    
    void WaitingForReceiveChange() // 等待玩家收錢 顧客才會離開
    {
        if(moneyObj.GetComponent<MoneyObj>().playerReceived)
        {
            CounterQueueManager.instance.RemoveFirst();
            CounterQueueManager.instance.UpdateQueue();
            ChangeState(NPCState.ExitStore);
            DestroyCheckedProducts();
            unCheckedProductsList.Clear();
        }
    }
    
    IEnumerator ExitStore() // NPC離開商店
    {
        if (!exitStoreFlag)
        {
            exitStoreFlag = true;
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
        StartCoroutine(SoundManager.instance.PlayClip_HurryUp());
        int randomRageTime = Random.Range(5, 10);
        yield return new WaitForSeconds(randomRageTime);
        _isRaging = false;
    }

    void CheckDaytime()
    {
        if (!DayNightManager.instance.isDaytime &&
            waypointNavigator.isInStore &&
            currentNpcState != NPCState.Checkout &&
            currentNpcState != NPCState.WaitingForCheckout &&
            currentNpcState != NPCState.Pay &&
            currentNpcState != NPCState.WaitingForReceiveChange)
        {
            waypointNavigator.isInStore = false;
            ChangeState(NPCState.ExitStore);
            DestroyCheckedProducts();
            unCheckedProductsList.Clear();
        }
    }

    void StateTree() // 行為狀態機
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
}
