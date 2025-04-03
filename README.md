# 🎮 One Receipt Away
### 👀 Download
  * [Itch.io](https://sean56787.itch.io/one-receipt-away)
  * [Google Drive](https://drive.google.com/drive/folders/1-oMF0VrzTKsHM9leTku_WUzo2oh1X_9R)
  * [完整Unity專案](https://drive.google.com/drive/folders/1hI6tAMe_-tIu1LNjzHz2Qra_1h8Xgvng)

## 📝 介紹
遊戲裡你將扮演一個打工人，在某個偏僻小鎮唯一的超商裡當收銀員，你不僅一早就要起來開店，幫客人結帳商品，有些人買一大堆東西害你手忙腳亂，有時還要忍受某些客人的壞脾氣，對你丟東西還出言羞辱，最慘的是就連睡覺也只能睡在儲藏室的地板，但為了生活，這些小事不算什麼，因為你有個夢想一直支撐著你，這個夢想就是用現金買下小鎮裡人人嚮往的那間溫馨小木屋

![Store](https://github.com/sean56787/TheRetrox/blob/main/README_IMG/%E5%95%86%E5%BA%97.png)

## 🖥️ 開發環境
| 名稱 | 版本 | 說明 |
|------|------|------|
| Unity | `2021.3.1f1 LTS` | 遊戲引擎 |
| C# | `10.0` | 主要開發語言 |
| Git | `2.37.1` | 版本控制 |

# 🔋 核心技術

## 玩家
| 配置 |
|------|
| `CapsuleCollider`, `Rigidbody` |

![玩家配置](https://github.com/sean56787/TheRetrox/blob/main/README_IMG/%E7%8E%A9%E5%AE%B6%E9%85%8D%E7%BD%AE.png)

| 行為 | 運作方式 |
|------|------|
| 玩家移動 | 使用`Rigidbody`的`Addforce`來推動玩家，可以表現出更加流暢和真實的移動 |
| 玩家視角 | 採第一人稱視角，屏幕中心白點用於拿取物品、與物件互動 |

## 物件
| 配置 |
|------|
| `MeshCollider` or `BoxCollider`, `Rigidbody` |

![物品配置](https://github.com/sean56787/TheRetrox/blob/main/README_IMG/%E7%89%A9%E5%93%81.png)

| 行為 | 運作方式 |
|------|------|
| 通用屬性 | 使用`IInteractable`這個`interface`來讓地圖物件有通用的屬性，像是`拿取`、`丟棄`、`使用`等，如果物品需要特殊功能再另外為它添加即可，大幅縮短開發時間 |
| 物件互動 | 依照`Camera`射出`Raycast`擊中的物件來檢測是否可互動，如物件有`interactable`的`Layer`即可拿起或互動 |
| 物體運動 | 設定為`Interpolate(插值)`，使物體運動更平滑、減少抖動 | 
| 碰撞檢測 | 設定為`Continuous(持續檢測碰撞)`，對於移動速度快的東西更能有效減少穿模的問題 |

## NPC 🤖
| 配置 |
|------|
| `CapsuleCollider`, `Rigidbody`, `NavMeshAgent` |

![NPC配置](https://github.com/sean56787/TheRetrox/blob/main/README_IMG/NPC%E9%85%8D%E7%BD%AE.png)

| 行為 | 運作方式 |
|------|------|
| NPC移動 | 使用Unity內建`NavMesh`來bake出NPC能行走的範圍，NPC移動時呼叫`NavMeshAgent.SetDestination`即可讓NPC移動至指定位置 |
| NPC狀態機 | `enum NPCState` 自訂NPC的16種狀態，並在遊戲中持續檢測狀態並執行對應動作，包括`行走`、`拿取物品`、`結帳`等|

## NavMesh
| 設置 |
|------|
| 將地形設置成`Navigation Static`後，在表面`Bake`出`NPC`行走範圍 |

![NavMesh](https://github.com/sean56787/TheRetrox/blob/main/README_IMG/NavMesh.png)

## NavMesh-Obstacle(空氣牆)
| 設置 |
|------|
| 使用`NavMesh`的`Obstacle`來限制`NPC`進入特定範圍 |

![Obstacle](https://github.com/sean56787/TheRetrox/blob/main/README_IMG/NavMesh-obstacle.png)

## Terrain
| 設置 |
|------|
| 地形設置使用內建`Terrain`系統快速搭建地形 |

![Terrain02](https://github.com/sean56787/TheRetrox/blob/main/README_IMG/%E5%9C%B0%E5%BD%A202.png)

| 設置 |
|------|
| 上色使用`Paint Texture` |

![Terrain02](https://github.com/sean56787/TheRetrox/blob/main/README_IMG/%E5%9C%B0%E5%BD%A203.png)

## 光源 ⛅
| 名稱 | 運作方式 |
|------|------|
| `Spot Light` | 模擬鎢絲燈泡 |

![Light01](https://github.com/sean56787/TheRetrox/blob/main/README_IMG/%E5%85%89%E6%BA%9001.png) 
![Light02](https://github.com/sean56787/TheRetrox/blob/main/README_IMG/%E5%85%89%E6%BA%9002.png)

| 名稱 | 運作方式 |
|------|------|
| `Point Light` | 模擬LED燈 |

![Light03](https://github.com/sean56787/TheRetrox/blob/main/README_IMG/%E5%85%89%E6%BA%9003.png)
![Light04](https://github.com/sean56787/TheRetrox/blob/main/README_IMG/%E5%85%89%E6%BA%9004.png)

| 名稱 | 配置 |
|------|------|
| 環境光源 | `Lighting` -> `Enviroment` 在白天時將`Ambient Color`設置成`0.6`，晚上則降為`0.1`，減少物體散發的自身光源，微小的`Reflection`讓物體反射`SkyBox`的光，夜晚可以模擬月光 |

![Light05](https://github.com/sean56787/TheRetrox/blob/main/README_IMG/%E7%92%B0%E5%A2%83%E5%85%8901.png)
![Light06](https://github.com/sean56787/TheRetrox/blob/main/README_IMG/%E7%92%B0%E5%A2%83%E5%85%8902.png)

## 其他
| 名稱 | 配置 |
|------|------|
| 霧氣 | `Lighting` -> `Enviroment` -> `Fog` 設為`Exponential`模式，讓場景帶有偏僻小鎮詭異的寧靜感|

![Fog](https://github.com/sean56787/TheRetrox/blob/main/README_IMG/%E9%9C%A7%E6%B0%A3.png)















