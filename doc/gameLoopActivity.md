# ゲームループ設計

ここでは、ゲームデモ内で1フレームごとに行う処理について説明します。  

ゲームデモ内では、主に下記を1フレームに一回行います。  

- ゲームデモ内の敵の数が一定以上になるように管理
- ゲームデモ内のアイテムの数が一定数になるように管理
- ボス敵の意思決定の更新

ゲームデモ内での1フレーム内での処理のフローを下記に示します。  
(アイテムの取得、敵の撃破処理はプレイヤークラス(PlayerManager)のイベント処理として行っています)  
  
```mermaid
flowchart TD
    node1["ゲームデモ開始・初期化"]
    node2["アイテムと敵の総数を確認"]
    node3["敵を再生成する"]
    node4["アイテムを再生成する"]

    node5{"アイテムの総数が<br>減っているか？"}
    node6{"敵の総数が<br>減っている、もしくは<br>一定時間経過したか？"}

    node7["キー入力を受け取り、プレイヤーキャラクターを動かす"]
    node8["ボス敵の意思決定の更新"]

    node1 --> node2
    node2 --> node5
    node5 -- Yes --> node4
    node4 --> node6
    node5 -- No --> node6
    node6 -- Yes --> node3
    node3 --> node7
    node6 -- No --> node7
    node7 --> node8
    node8 --> node2
```

ゲームデモ内の敵の数とアイテムの数は、ゲーム全体を管理するGameManagerクラスがメンバとして持つ、AI_metaクラスで行います。  
また、ボス敵の意思決定は[こちら](/doc/bossAI.md)で説明しています。  
  
## メタAIクラス

メタAIクラスは、ゲームループの中で下記の機能を持っています。  

- アイテムの総数が一定になるように管理
- 一定時間経過するまでは敵の数も一定になるように管理
- 一定時間経過した場合は敵が増え続ける
    
具体的には、unityのMonobehaviorクラスのUpdate関数でアイテムと敵が減ったかを確認し、減った分だけ敵もしくはアイテムを再生成します。  

クラスに関しては[こちら](/doc/class.md)で説明しています。  





