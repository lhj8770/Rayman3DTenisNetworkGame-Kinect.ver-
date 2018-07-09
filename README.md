# Rayman3DTenisNetworkGame-Kinect.ver-

私と友達の二人チームで大学卒業作品(capstone)として関節動作認識機器であるキネクトを使って二人用ネットワークテニスゲームを作りました。

作業分担は私がキネクトとUNITYのグラウンドとネットワークを担当して友達がグラフィックとラケットを担当しました。

ラケットの方は最初はキネクトで表現する予定でしたが関節認識だけでは手の自然な動きを表現することには及ばないと思って
携帯電話のジャイロセンサーをブルートゥース通信でUNITYに送ってそのセンサーの値をそのままラケットのQUATERNION表現に適用しました。

関節認識は合わせて21個の点を認識しますがゲームをネットワーク化するためにはサーバーに送る情報の数が多すぎにならないようにする必要が
あってキャラクターを1：1のヒューマノイドではないノン - ヒューマノイドキャラクターに変更するアイディアを考えて
送る関節のvector情報を6個まで減らしました。その中で変更した３D　raymanキャラクターが人と関節の比率が違ったのでもともと膝だった点をキャラクターの足の動きに適用したり倍率をゲームの中で掛けることでゲームのプレイが可能な形に変えました。(UnitController.cs)

ジェスチャーは計画ではラケットを振ることもふくめて動作一つ一つをジェスチャー化する計画でしたが左手を上げてゲームを始めるだけの機能だけを使うようにしました。（GestureLister.cs）

