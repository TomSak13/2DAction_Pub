# ゲームデモ概要

- プレイヤーキャラクターであるキツネを操作し、敵を倒していくゲームデモです。
- 敵には種類があり、雑魚敵とボス敵がいます。左上のHPゲージが無くなる前にボス敵を倒せば、このゲームデモはクリアとなります。

![プレイ画面サンプル](/doc/img/playSampleforDescription.png)

- 雑魚敵はキツネが上から踏みつけることによって倒せます

![敵撃破サンプル](/doc/img/playSampleforEnemyDestroy.png)


- ただし、ボス敵は一度踏みつけるだけでは倒すことができません。<br>ボス敵は踏みつけると右上のHPゲージが減っていき、このゲージが0になるとボス敵を倒すことができます。

![ボスサンプル](/doc/img/playSampleforBOSS.png)

- プレイヤーは上から踏む以外に敵に接触すると左上のHPゲージが減っていきますが、フィールドに落ちているアイテムと接触するとHPのゲージが増えます。

![アイテムサンプル1](/doc/img/playSampleforItem.png) ![アイテムサンプル2](/doc/img/playSampleforItem2.png)

- また、ゲームデモ内にある棘のオブジェクトに接触したり、フィールドから落下した場合は即ゲームオーバーとなります

![ゲームオーバーサンプル](/doc/img/playSampleforGameOver.png)

# ゲームデモ操作方法

ゲームデモ内での操作キーの割り当てについて下記に示します。

| キー | ゲームデモ内での操作 |
| --- | --- |
| 右十字キー | プレイヤーが右へ動く |
| 左十字キー | プレイヤーが左へ動く |
| スペースキー | プレイヤーがジャンプする。押したままにする<br>ことでジャンプ距離が一定距離まで延びる |

# ゲームデモ設計・開発環境

## 開発環境

| 項目 | 仕様環境 | バージョン |
| --- | --- | --- |
| OS | windows | Windows10 | 
| ゲームエンジン | Unity | 2021.3.14f1 |
| ビルド環境 | Visual Studio | Microsoft Visual Studio Community 2019 |  


## 設計

このゲームデモに関するシステム設計とスクリプトに関してここで説明します。

このゲームデモにおける、おおよそ1フレーム内に行う処理のループ(ゲームループ)のアクティビティ図を下記に示します。

- [ゲームループ設計](/doc/gameLoopActivity.md)

加えて各スクリプトに関して、下記のリンク先に示します。

- [スクリプト(クラス)設計](/doc/class.md)

また、ボス敵はステートマシンによる行動切り替えを行っています。その設計を下記に示します。

- [ボス敵設計](/doc/bossAI.md)


## 使用素材

- プレイヤー、タイルマップ、雑魚敵素材
https://assetstore.unity.com/packages/2d/characters/sunny-land-103349

- BOSS素材
https://assetstore.unity.com/packages/2d/characters/monsters-creatures-fantasy-167949

- 音楽(BGM・SE)素材
https://maou.audio/