using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class KinectAvatar : MonoBehaviour {

    [SerializeField]
    Camera cam;

    [SerializeField]
    Vector3 leftLegPos; //左脚
    [SerializeField]
    Vector3 leftUpLegPos;//左膝

    [SerializeField]
    Vector3 rightLegPos;//右脚
    [SerializeField]
    Vector3 rightUpLegPos;//右膝

    [SerializeField]
    Vector3 spine1Pos;//脊椎
    [SerializeField]
    Vector3 spine2Pos;//脊椎２

    [SerializeField]
    Vector3 hipsPos;//腰だと思われる
    [SerializeField]
    Vector3 leftHipPos;//左腰
    [SerializeField]
    Vector3 rightHipPos;//右腰

    [SerializeField]
    Vector3 leftShoulderPos;//左肩
    [SerializeField]
    Vector3 rightShoulderPos;//右肩

    [SerializeField]
    Vector3 leftArmPos;//左肘
    [SerializeField]
    Vector3 leftForeArmPos;//左手首
    [SerializeField]
    Vector3 leftHandPos;//左手

    [SerializeField]
    Vector3 rightArmPos;//右肘
    [SerializeField]
    Vector3 rightForeArmPos;//右手首
    [SerializeField]
    Vector3 rightHandPos;//右手

    [SerializeField]
    public int NewPose;//現在のポーズ

    //テキスト表示するため
    public Text text;

    //鏡にするかそのままにするかの判定
    public bool IsMirror = true;
    public int[] Poze=new int[4];//糞配列
    public int chack ;//どのポーズが出来ていないか
    public int Result ;//ポーズがちゃんと出来ている
    public int PozeCount;
    //現在のポーズを格納　
    
    //ほかのスクリプトの値を使いたいのでここで参照かな？
    public BodySourceManager _BodyManager;
    public GameObject _UnityChan;
    //public GameSystemScript GSSript;

    //GameObjを取得するための定義
    public GameObject Ref;
    public GameObject Hips;
    public GameObject RightHips;
    public GameObject LeftHips;
    public GameObject LeftUpLeg;
    public GameObject LeftLeg;
    public GameObject RightUpLeg;
    public GameObject RightLeg;
    public GameObject Spine1;
    public GameObject Spine2;
    public GameObject LeftShoulder;
    public GameObject LeftArm;
    public GameObject LeftForeArm;
    public GameObject LeftHand;
    public GameObject RightShoulder;
    public GameObject RightArm;
    public GameObject RightForeArm;
    public GameObject RightHand;
    public GameObject Neck;
    public GameObject Head;


    //最初だ読み込む必要のあるものを表記
    void Start() {
        Result = 0;

        //ポーズの初期値を設定
        for(int i=0;i<4;i++)//ここの　i　<　○　の数値は登録してあるポーズにする
        {
            Poze[i] = i;
        }


        //ここでモデルをベースに各関節のGameObjを取得
        Ref =
            _UnityChan.transform.FindChild("Character1_Reference").gameObject;
        //真ん中の腰
        Hips =
            Ref.gameObject.transform.FindChild("Character1_Hips").gameObject;
        //欠陥品
        ////右の腰
        //RightHips =
        //    Hips.transform.FindChild("Character1_RightHips").gameObject;
        ////左の腰
        //LeftHips=
        //    Hips.transform.FindChild("Character1_RightHips").gameObject;
        //左の脚　上
        LeftUpLeg =
            Hips.transform.FindChild("Character1_LeftUpLeg").gameObject;
        //左の脚　
        LeftLeg =
            LeftUpLeg.transform.FindChild("Character1_LeftLeg").gameObject;
        //右の脚　上
        RightUpLeg =
            Hips.transform.FindChild("Character1_RightUpLeg").gameObject;
        //右の脚
        RightLeg =
            RightUpLeg.transform.FindChild("Character1_RightLeg").gameObject;
        //脊椎判定１
        Spine1 =
            Hips.transform.FindChild("Character1_Spine").
                    gameObject.transform.FindChild("Character1_Spine1").gameObject;
        //脊椎判定２
        Spine2 =
            Spine1.transform.FindChild("Character1_Spine2").gameObject;
        //左の肩の判定
        LeftShoulder =
            Spine2.transform.FindChild("Character1_LeftShoulder").gameObject;
        //左の腕の判定
        LeftArm =
            LeftShoulder.transform.FindChild("Character1_LeftArm").gameObject;
        //左の腕の前の判定
        LeftForeArm =
            LeftArm.transform.FindChild("Character1_LeftForeArm").gameObject;
        //左の手の判定
        LeftHand =
            LeftForeArm.transform.FindChild("Character1_LeftHand").gameObject;
        //右の肩の判定
        RightShoulder =
            Spine2.transform.FindChild("Character1_RightShoulder").gameObject;
        //右の腕の判定
        RightArm =
            RightShoulder.transform.FindChild("Character1_RightArm").gameObject;
        //右の前の腕の判定
        RightForeArm =
            RightArm.transform.FindChild("Character1_RightForeArm").gameObject;
        //右の手の判定
        RightHand =
            RightForeArm.transform.FindChild("Character1_RightHand").gameObject;
        //首の判定
        Neck =
            Spine2.transform.FindChild("Character1_Neck").gameObject;
        //頭の判定
        Head =
            Neck.transform.FindChild("Character1_Head").gameObject;

        
    }
    // フレーム毎にループして入る
    void Update()
    {
        //これはBodyになにも値が入ってないときにまあさよならと
        if (_BodyManager == null)
        {
            Debug.Log("_BodyManager == null");
            return;
        }

        // Bodyデータを取得する
        var data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }

        // 最初に追跡している人を取得する
        var body = data.FirstOrDefault(b => b.IsTracked);
        if (body == null)
        {
            return;
        }

        // 床の傾きを取得する
        //floorPlaneに_Bodymanagerの中にあるFloorClipPlaneを取得する
        var floorPlane = _BodyManager.FloorClipPlane;

        var comp = Quaternion.FromToRotation(
            new Vector3(floorPlane.X, floorPlane.Y, floorPlane.Z), Vector3.up);

        // 関節の回転を取得する
        var joints = body.JointOrientations;

        //ここで関節の角度をQuaternionに変換する
        Quaternion SpineBase;
        //Quaternion HipRight;//爆弾投下なり
        //Quaternion HipLeft;//馬鹿が馬鹿なりに追加してみた
        Quaternion SpineMid;
        Quaternion SpineShoulder;
        Quaternion ShoulderLeft;
        Quaternion ShoulderRight;
        Quaternion ElbowLeft;
        Quaternion WristLeft;
        Quaternion HandLeft;
        Quaternion ElbowRight;
        Quaternion WristRight;
        Quaternion KneeLeft;
        Quaternion HandRight;
        Quaternion AnkleLeft;
        Quaternion KneeRight;
        Quaternion AnkleRight;

        // 鏡の時の判定の仕方　右のものを左に格納　左に右を
        if (IsMirror) {
            SpineBase = joints[JointType.SpineBase].Orientation.ToMirror().ToQuaternion(comp);
            //HipRight= joints[JointType.HipRight].Orientation.ToMirror().ToQuaternion(comp);//起爆
            //HipLeft= joints[JointType.HipLeft].Orientation.ToMirror().ToQuaternion(comp);//成功してくれー（懇願
            SpineMid = joints[JointType.SpineMid].Orientation.ToMirror().ToQuaternion(comp);
            SpineShoulder = joints[JointType.SpineShoulder].Orientation.ToMirror().ToQuaternion(comp);
            ShoulderLeft = joints[JointType.ShoulderRight].Orientation.ToMirror().ToQuaternion(comp);
            ShoulderRight = joints[JointType.ShoulderLeft].Orientation.ToMirror().ToQuaternion(comp);
            ElbowLeft = joints[JointType.ElbowRight].Orientation.ToMirror().ToQuaternion(comp);
            WristLeft = joints[JointType.WristRight].Orientation.ToMirror().ToQuaternion(comp);
            HandLeft = joints[JointType.HandRight].Orientation.ToMirror().ToQuaternion(comp);
            ElbowRight = joints[JointType.ElbowLeft].Orientation.ToMirror().ToQuaternion(comp);
            WristRight = joints[JointType.WristLeft].Orientation.ToMirror().ToQuaternion(comp);
            HandRight = joints[JointType.HandLeft].Orientation.ToMirror().ToQuaternion(comp);
            KneeLeft = joints[JointType.KneeRight].Orientation.ToMirror().ToQuaternion(comp);
            AnkleLeft = joints[JointType.AnkleRight].Orientation.ToMirror().ToQuaternion(comp);
            KneeRight = joints[JointType.KneeLeft].Orientation.ToMirror().ToQuaternion(comp);
            AnkleRight = joints[JointType.AnkleLeft].Orientation.ToMirror().ToQuaternion(comp);
        }
        // kinectで読み込んだものを反転させずそのまま使用
        else {
            SpineBase = joints[JointType.SpineBase].Orientation.ToQuaternion(comp);
            //HipRight = joints[JointType.HipLeft].Orientation.ToMirror().ToQuaternion(comp);//起爆
            //HipLeft = joints[JointType.HipRight].Orientation.ToMirror().ToQuaternion(comp);//成功してくれー（懇願
            SpineMid = joints[JointType.SpineMid].Orientation.ToQuaternion(comp);
            SpineShoulder = joints[JointType.SpineShoulder].Orientation.ToQuaternion(comp);
            ShoulderLeft = joints[JointType.ShoulderLeft].Orientation.ToQuaternion(comp);
            ShoulderRight = joints[JointType.ShoulderRight].Orientation.ToQuaternion(comp);
            ElbowLeft = joints[JointType.ElbowLeft].Orientation.ToQuaternion(comp);
            WristLeft = joints[JointType.WristLeft].Orientation.ToQuaternion(comp);
            HandLeft = joints[JointType.HandLeft].Orientation.ToQuaternion(comp);
            ElbowRight = joints[JointType.ElbowRight].Orientation.ToQuaternion(comp);
            WristRight = joints[JointType.WristRight].Orientation.ToQuaternion(comp);
            HandRight = joints[JointType.HandRight].Orientation.ToQuaternion(comp);
            KneeLeft = joints[JointType.KneeLeft].Orientation.ToQuaternion(comp);
            AnkleLeft = joints[JointType.AnkleLeft].Orientation.ToQuaternion(comp);
            KneeRight = joints[JointType.KneeRight].Orientation.ToQuaternion(comp);
            AnkleRight = joints[JointType.AnkleRight].Orientation.ToQuaternion(comp);
        }
        // 関節の回転を計算する
        Quaternion q = transform.rotation;
        transform.rotation = Quaternion.identity;
        
        var comp2 = Quaternion.AngleAxis(90, new Vector3(0, 1, 0)) *
                    Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));

        Spine1.transform.rotation = SpineMid * comp2;
        //右腕
        RightArm.transform.rotation = ElbowRight * comp2;
        RightForeArm.transform.rotation = WristRight * comp2;
        RightHand.transform.rotation = HandRight * comp2;
        //左腕
        LeftArm.transform.rotation = ElbowLeft * comp2;
        LeftForeArm.transform.rotation = WristLeft * comp2;
        LeftHand.transform.rotation = HandLeft * comp2;
        //右脚
        //RightHips.transform.rotation = HipRight * comp2;
        RightUpLeg.transform.rotation = KneeRight * comp2;
        RightLeg.transform.rotation = AnkleRight * comp2;
        //左脚
        //LeftHips.transform.rotation = HipLeft * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        LeftUpLeg.transform.rotation = KneeLeft * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        LeftLeg.transform.rotation = AnkleLeft * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));

        // モデルの回転を設定する
        transform.rotation = q;

        // モデルの位置を移動する
        var pos = body.Joints[JointType.SpineMid].Position;
        Ref.transform.position = new Vector3(-pos.X, pos.Y, -pos.Z);

       

        //ここで判定をとるタイミングを指定今は作ってない
        //またchackで判定とれているか判別しているためうまく判定するときを設定する必要あり

        
        //エンターを押したときのみ判断　のちに時間で判定したい
        if(Input.GetKey(KeyCode.Return))
        {
            //ここでポーズの値を初期化次の判定でミスした時のため
            //本当はelseに入れたい
            PozeCount = 0;

            //ここでカメラからみた座標を格納している
            leftLegPos = cam.WorldToScreenPoint(LeftLeg.transform.position);
            rightLegPos = cam.WorldToScreenPoint(RightLeg.transform.position);
            spine1Pos = cam.WorldToScreenPoint(Spine1.transform.position);
            leftHandPos = cam.WorldToScreenPoint(LeftHand.transform.position);
            leftUpLegPos = cam.WorldToScreenPoint(LeftUpLeg.transform.position);
            rightUpLegPos = cam.WorldToScreenPoint(RightUpLeg.transform.position);
            spine2Pos = cam.WorldToScreenPoint(Spine2.transform.position);
            hipsPos = cam.WorldToScreenPoint(Hips.transform.position);
            //leftHipPos= cam.WorldToScreenPoint(LeftHips.transform.position);//爆弾　解放厳禁
            //rightHipPos= cam.WorldToScreenPoint(RightHips.transform.position);
            leftShoulderPos = cam.WorldToScreenPoint(LeftShoulder.transform.position);
            rightShoulderPos = cam.WorldToScreenPoint(RightShoulder.transform.position);
            leftArmPos = cam.WorldToScreenPoint(LeftArm.transform.position);
            leftForeArmPos = cam.WorldToScreenPoint(LeftForeArm.transform.position);
            leftHandPos = cam.WorldToScreenPoint(LeftHand.transform.position);
            rightArmPos = cam.WorldToScreenPoint(RightArm.transform.position);
            rightForeArmPos = cam.WorldToScreenPoint(RightForeArm.transform.position);
            rightHandPos = cam.WorldToScreenPoint(RightHand.transform.position);

            //ポーズ判定処理
            //まず足の位置で大雑把にポーズを分けたい
            //なので足の間の座標間が２０以上なら足が開いているとする
            //流れ　足の間隔→左腕の判定→右腕の判定　としている
            //脚は一定範囲開いていた//なお左膝より左脚、右膝より右脚の方が体より遠く


            
            //メインで指定したポーズのみ成功しているかの判定
            //デバック用UI表示込み　判定が成功していれば表示される

            switch (NewPose)
            {
                case 0:
                    text.text = "何も入っていません";
                    Result = 2;
                    break;
                //現在のポーズ
                case 1://ヒーローらしい？　20は座標的に脚を開いている大きさとする
                    if ((leftLegPos - rightLegPos).x > 20 && leftUpLegPos.x < leftLegPos.x
                        && rightUpLegPos.x > rightLegPos.x && leftForeArmPos.x > leftHandPos.x
                         && leftForeArmPos.y > leftHandPos.y && leftShoulderPos.y > leftForeArmPos.y
                          && leftForeArmPos.x > leftShoulderPos.x && rightForeArmPos.x < rightHandPos.x
                           && rightForeArmPos.y > rightHandPos.y && rightForeArmPos.x < rightShoulderPos.x
                            && rightShoulderPos.y > rightForeArmPos.y)
                    { text.text = "ヒーロー参上である！！"; Result = 1; }
                    else
                    { Result = 2; }
                    break;
                case 2://大文字 -10と10は誤差範囲です
                    if ((leftLegPos - rightLegPos).x > 20 && leftUpLegPos.x < leftLegPos.x
                        && rightUpLegPos.x > rightLegPos.x && (leftHandPos - leftForeArmPos).y < 10
                         && (leftHandPos - leftForeArmPos).y > -10 && (leftShoulderPos - leftForeArmPos).y < 10
                          && (leftHandPos - leftForeArmPos).y > -10 && (rightHandPos - rightForeArmPos).y < 10
                           && (rightHandPos - rightForeArmPos).y > -10 && (rightShoulderPos - rightForeArmPos).y < 10
                            && (rightHandPos - rightForeArmPos).y > -10)
                    { text.text = "大の字成功でやんす"; Result = 1; }
                    else
                    { Result = 2; }
                    break;
                case 3://Y字　よくわかりませんね
                    if ((leftLegPos - rightLegPos).x <= 20 && leftShoulderPos.x < leftForeArmPos.x
                        && leftForeArmPos.x < leftHandPos.x && leftShoulderPos.y < leftForeArmPos.y
                         && leftForeArmPos.y < leftHandPos.y && rightShoulderPos.x > rightForeArmPos.x
                          && rightForeArmPos.x > rightHandPos.x && rightShoulderPos.y < rightForeArmPos.y
                           && rightForeArmPos.y < rightHandPos.y)
                    { text.text = "ロムルスーーーーーーーーー"; Result = 1; }
                    else
                    { Result = 2; }
                    break;
                case 4://２０は脚を開いていると過程するもの　
                    if ((leftLegPos - rightLegPos).x > 20 && leftUpLegPos.x < leftLegPos.x
                        /*&& rightUpLegPos.x > rightLegPos.x*/ && leftUpLegPos.y < leftLegPos.y
                        && leftShoulderPos.x < leftArmPos.x && leftArmPos.x < leftForeArmPos.x
                         && leftArmPos.y > leftForeArmPos.y && rightShoulderPos.y > rightForeArmPos.y
                          && rightShoulderPos.x > rightArmPos.x && rightArmPos.x > rightForeArmPos.x)
                    { text.text = "非常口"; Result = 1; }
                    else
                    { Result = 2; }
                    break;
                case 5:
                    //腕のみで判定をとっています　足まで入れる必要があるポーズでもないかと
                    if (leftShoulderPos.x < leftArmPos.x && leftArmPos.x > leftForeArmPos.x
                        && leftShoulderPos.y < leftArmPos.y && leftArmPos.y < leftForeArmPos.y
                         && rightShoulderPos.x < rightArmPos.x && rightArmPos.x > rightForeArmPos.x
                          && rightShoulderPos.y < rightArmPos.y && rightArmPos.y < rightForeArmPos.y)
                    { text.text = "挨拶は大事ですね"; Result = 1; }
                    else
                    { Result = 2; }

                    break;
                case 6://40は脚の間隔の狭さ　もっと狭くしてもいいかも　要調整
                    if ((leftLegPos - rightLegPos).x <= 40 && rightUpLegPos.x > rightLegPos.x
                        && leftUpLegPos.x > leftLegPos.x && leftShoulderPos.y > leftArmPos.y
                         && leftArmPos.y > leftForeArmPos.y && leftShoulderPos.x < leftArmPos.x
                          && leftArmPos.x < leftForeArmPos.x && rightShoulderPos.y > rightArmPos.y
                           && rightArmPos.y > rightForeArmPos.y && rightShoulderPos.x < rightArmPos.x
                            && rightArmPos.x < rightForeArmPos.x)
                    { text.text = "ヤンキー座り？"; Result = 1; }
                    else
                    { Result = 2; }
                    break;
                case 7:
                    if (leftShoulderPos.x < leftArmPos.x && leftArmPos.x > leftForeArmPos.x
                        && leftShoulderPos.y > leftArmPos.y && leftArmPos.y < leftForeArmPos.y
                         && rightShoulderPos.x > rightArmPos.x && rightArmPos.x > rightForeArmPos.x
                          && rightShoulderPos.y < rightArmPos.y && rightArmPos.y < rightForeArmPos.y)
                    { text.text = "ヒーローだぁー(｀･ω･´)"; Result = 1; }
                    else
                    { Result = 2; }
                    break;
                case 8:
                    if ((leftLegPos - rightLegPos).x <= 20 && leftShoulderPos.x > leftArmPos.x
                        && leftArmPos.x > leftForeArmPos.x && leftShoulderPos.y < leftArmPos.y
                         && rightShoulderPos.x > rightArmPos.x && rightArmPos.x < rightForeArmPos.x
                          && rightShoulderPos.y > rightArmPos.y && (rightArmPos - rightForeArmPos).y <= 20
                           && (rightArmPos - rightForeArmPos).y >= -20)
                    { text.text = "ラジオ体操第一、、"; Result = 1; }
                    else
                    { Result = 2; }

                    break;
                case 9:
                    if (leftShoulderPos.x < leftArmPos.x && leftArmPos.x > leftForeArmPos.x
                        && (leftShoulderPos - leftArmPos).y <= 10 && (leftShoulderPos - leftArmPos).y >= -10
                         && leftArmPos.y < leftForeArmPos.y && rightShoulderPos.x > rightArmPos.x
                          && rightArmPos.x < rightForeArmPos.x && (rightShoulderPos - rightArmPos).y <= 10
                           && (rightShoulderPos - rightArmPos).y >= -10 && rightArmPos.y < rightForeArmPos.y)
                    { text.text = "マッスル　むきむき"; Result = 1; }
                    else
                    { Result = 2; }

                    break;
                case 10://10は、誤差範囲　２０は脚の間隔２０以下
                    if ((leftLegPos - rightLegPos).x <= 20 && leftShoulderPos.x < leftArmPos.x
                        && (leftArmPos - leftForeArmPos).x < 10 && (leftArmPos - leftForeArmPos).x > -10
                         && (leftShoulderPos - leftArmPos).y < 10 && (leftShoulderPos - leftArmPos).y > -10
                          && leftArmPos.y > leftForeArmPos.y && (rightArmPos - rightForeArmPos).x > -10
                           && (rightShoulderPos - rightArmPos).y < 10 && (rightShoulderPos - rightArmPos).y > -10
                            && rightArmPos.y < rightForeArmPos.y)
                    { text.text = "はにわ"; Result = 1; }
                    else
                    { Result = 2; }
                    break;
                case 11:
                    if (leftUpLegPos.x > leftLegPos.x && rightUpLegPos.x > rightLegPos.x
                        && leftArmPos.x < leftForeArmPos.x && leftShoulderPos.y > leftArmPos.y
                         && rightArmPos.x < rightForeArmPos.x && rightShoulderPos.y < rightArmPos.y)
                    { text.text = "合体だぁーー"; }
                    break;
                case 12:
                    if ((leftLegPos - rightLegPos).x <= 20 && leftShoulderPos.x < leftArmPos.x
                        && leftArmPos.x < leftForeArmPos.x && leftShoulderPos.y < leftArmPos.y
                         && leftArmPos.y < leftForeArmPos.y && rightShoulderPos.x > rightArmPos.x
                          && rightArmPos.x > rightForeArmPos.x && rightShoulderPos.y < rightArmPos.y
                           && rightArmPos.y < rightForeArmPos.y)
                    { text.text = "あぁー神よ、崇め奉りたまえ！！"; Result = 1; }
                    else
                    { Result = 2; }

                    break;
                case 13:
                    if (leftUpLegPos.x > leftLegPos.x && rightUpLegPos.x > rightLegPos.x
                        && leftShoulderPos.x < leftArmPos.x && leftArmPos.x < leftForeArmPos.x
                         && leftShoulderPos.y < leftArmPos.y && leftArmPos.y < leftForeArmPos.y
                          && rightShoulderPos.x > rightArmPos.x && rightArmPos.x < rightForeArmPos.x
                           && rightShoulderPos.y > rightArmPos.y && rightArmPos.y < rightForeArmPos.y)
                    { text.text = "一発屋のポーズ"; Result = 1; }
                    else
                    { Result = 2; }
                    
                        break;
                case 14://20は脚の間隔
                    if ((leftLegPos - rightLegPos).x > 20 && leftUpLegPos.x < leftLegPos.x
                        && rightUpLegPos.x > rightLegPos.x && rightShoulderPos.x > rightArmPos.x
                         && rightArmPos.x < rightForeArmPos.x && rightShoulderPos.y < rightArmPos.y
                          && rightForeArmPos.y > rightArmPos.y)
                    { text.text = "勝利のポーズ！！"; Result = 1; }
                    else
                    { Result = 2; }
                    break;
                case 15://20は脚の間隔
                    if ((leftLegPos - rightLegPos).x > 20 && leftUpLegPos.x < leftLegPos.x
                        && rightUpLegPos.x < rightLegPos.x && leftShoulderPos.x < leftArmPos.x
                         && leftArmPos.x < leftForeArmPos.x && leftShoulderPos.y > leftArmPos.y
                          && leftArmPos.y > leftForeArmPos.y && rightShoulderPos.x > rightArmPos.x
                           && rightArmPos.x < rightForeArmPos.x && rightShoulderPos.y > rightArmPos.y
                            && rightArmPos.y > rightForeArmPos.y)
                    { text.text = "屈伸、";Result = 1; }
                    else
                    { Result = 2; }
                    
                        break;
                case 16:
                    if (leftUpLegPos.x > leftLegPos.x && leftUpLegPos.y > leftLegPos.y
                        && (leftHandPos - leftForeArmPos).y < 10 && (leftHandPos - leftForeArmPos).y > -10 
                         && (leftShoulderPos - leftForeArmPos).y < 10 && (leftHandPos - leftForeArmPos).y > -10 
                          && (rightHandPos - rightForeArmPos).y < 10 && (rightHandPos - rightForeArmPos).y > -10 
                           && (rightShoulderPos - rightForeArmPos).y < 10 && (rightHandPos - rightForeArmPos).y > -10)
                    { text.text = "バランス"; Result = 1; }
                    else
                    { Result = 2; }

                    break;
                case 17://交差あり
                    if((leftShoulderPos-rightLegPos).x <=-5 && leftShoulderPos.x <leftArmPos.x
                        && leftArmPos.x <leftForeArmPos.x && leftShoulderPos.y > leftArmPos.y 
                         && leftArmPos.y > leftForeArmPos.y && rightShoulderPos.x > rightArmPos.x
                          && rightArmPos.x > rightForeArmPos.x && rightShoulderPos.y > rightArmPos.y
                           && rightArmPos.y > rightForeArmPos.y)
                    { text.text = "オサレ"; Result = 1; }
                    else
                    { Result = 2; }

                    break;
                case 18:
                    if((leftLegPos - rightLegPos).x > 20 && leftUpLegPos.x < leftLegPos.x
                        && rightUpLegPos.x < rightLegPos.x && leftShoulderPos.x < leftArmPos.x
                         && leftShoulderPos.y < leftArmPos.y && leftArmPos.y < leftForeArmPos.y
                          && rightShoulderPos.x > rightArmPos.x && rightShoulderPos.y < rightArmPos.y
                           && rightArmPos.y < rightForeArmPos.y)
                    { text.text = "荷物運び"; Result = 1; }
                    else
                    { Result = 2; }

                    break;
                case 19:
                    if (leftUpLegPos.x < leftLegPos.x && leftUpLegPos.y > leftLegPos.y
                        && rightUpLegPos.x < rightLegPos.x && rightUpLegPos.y > rightLegPos.y
                         && leftShoulderPos.x < leftArmPos.x && leftArmPos.x <leftForeArmPos.x
                          && leftShoulderPos.y <leftArmPos.y && leftArmPos.y < leftForeArmPos.y
                           && rightShoulderPos.x > rightArmPos.x && rightArmPos.x < rightForeArmPos.x
                            && rightShoulderPos.y > rightArmPos.y && rightArmPos.y < rightForeArmPos.y)
                    { text.text = "勝者の構え"; Result = 1; }
                    else
                    { Result = 2; }
                    break;
                case 20://腕は肘が一番低い位置として指定
                    if(leftUpLegPos.x >leftLegPos.x && leftUpLegPos.y > leftLegPos.y 
                        && rightUpLegPos.x < rightLegPos.x && rightUpLegPos.y > rightLegPos.y
                         && leftShoulderPos.x <leftArmPos.x && leftArmPos.x < leftForeArmPos.x
                          && leftShoulderPos.y > leftArmPos.y && leftArmPos.y < leftForeArmPos.y
                           && rightShoulderPos.x > rightArmPos.x && rightArmPos.x> rightForeArmPos.x
                            && rightShoulderPos.y > rightArmPos.y && rightArmPos.y < rightForeArmPos.y)
                    { text.text = "大魔王"; Result = 1; }
                    else
                    { Result = 2; }
                        
                    break;
                case 21:
                    if(leftUpLegPos.x > leftLegPos.x && leftUpLegPos.y > leftLegPos.y
                        && rightUpLegPos.x < rightLegPos.x && rightUpLegPos.y > rightLegPos.y
                         && leftShoulderPos.x < leftArmPos.x && leftArmPos.x >leftForeArmPos.x
                          && leftShoulderPos.y > leftArmPos.y && leftArmPos.y > leftForeArmPos.y 
                           && rightShoulderPos.x > leftArmPos.x && leftArmPos.x > leftForeArmPos.x
                            && leftShoulderPos.y > leftArmPos.y && leftArmPos.y < leftForeArmPos.y)
                    { text.text = "どすこい"; Result = 1; }
                    else
                    { Result = 2; }

                    break;
                case 22://１０は地面よりある程度上げていることを示す
                    if(leftUpLegPos.x > leftLegPos.x && leftUpLegPos.y > leftLegPos.y
                        && rightUpLegPos.x < rightLegPos.x && rightUpLegPos.y > rightLegPos.y
                         && rightLegPos.y + 10 < leftLegPos.y && leftShoulderPos.x <leftArmPos.x
                          && leftArmPos.x <leftForeArmPos.x && leftShoulderPos.y >leftArmPos.y 
                           && leftArmPos.y <leftForeArmPos.y && rightShoulderPos.x <rightArmPos.x
                            && rightArmPos.x < rightForeArmPos.x && rightShoulderPos.y < rightArmPos.y 
                             && rightArmPos.y <rightForeArmPos.y)
                    { text.text = "みなぎってきたぁー"; Result = 1; }

                    break;
                case 23:
                    if(leftUpLegPos.x < leftLegPos.x && leftUpLegPos.y < leftLegPos.y
                        && leftShoulderPos.x < leftArmPos.x && leftArmPos.x > leftForeArmPos.x
                         && (leftShoulderPos - leftArmPos).y <= 10 && (leftShoulderPos - leftArmPos).y >= -10
                          && leftArmPos.y <leftForeArmPos.y && rightShoulderPos.x > rightArmPos.x 
                           && rightArmPos.x < rightForeArmPos.x && (rightShoulderPos - rightArmPos).y <= 10 
                            && (rightShoulderPos - rightArmPos).y >= -10 && rightArmPos.y < rightForeArmPos.y)
                    { text.text = "アイドル";Result = 1; }
                    else
                    { Result = 2; }

                    break;
                case 24:
                    if(leftUpLegPos.x <leftLegPos.x && leftUpLegPos.y > leftLegPos.y 
                        && rightUpLegPos.x < rightLegPos.x && rightUpLegPos.y > rightLegPos.y
                         && rightLegPos.y + 5 < leftLegPos.y && leftShoulderPos.x < leftArmPos.x 
                          && leftArmPos.x > leftForeArmPos.x && leftShoulderPos.y < leftArmPos.y
                           && leftArmPos.y < leftForeArmPos.y && rightShoulderPos.x > rightArmPos.x
                            && rightArmPos.x < rightForeArmPos.x && rightShoulderPos.y < rightArmPos.y
                             && rightArmPos.y < rightForeArmPos.y)
                    { text.text = "芸能人の舞"; Result = 1; }
                    else
                    { Result = 2; }
                    
                    break;
                case 25:
                    if(leftUpLegPos.x < leftLegPos.x && (leftUpLegPos - leftLegPos).y <= 10
                        && (leftUpLegPos-leftLegPos).y >= -10 && rightUpLegPos.x < rightLegPos.x
                         && rightUpLegPos.y > rightLegPos.y && leftShoulderPos.x <leftArmPos.x
                          && leftArmPos.x >leftForeArmPos.x && (leftShoulderPos-leftForeArmPos).y <= 10
                           && (leftShoulderPos-leftForeArmPos).y >= -10 && rightShoulderPos.x > rightArmPos.x
                            && rightArmPos.x< rightForeArmPos.x && rightShoulderPos.y < rightArmPos.y 
                             && rightArmPos.y <rightForeArmPos.y)
                    { text.text = "マッスル"; Result = 1; }
                    else
                    { Result = 2; }

                    break;
                case 26://交差してしまうと全く違うものになるので１０から０の範囲に
                    if((leftLegPos- rightLegPos).x <= 10 && (leftLegPos-rightLegPos).x >=0
                        && (leftUpLegPos - rightUpLegPos).x <= 10 && (leftUpLegPos - rightUpLegPos).x >= 0
                         && leftShoulderPos.x < leftArmPos.x && leftArmPos.x < leftForeArmPos.x
                          && leftShoulderPos.y > leftArmPos.y && leftArmPos.y > leftForeArmPos.y
                           && rightShoulderPos.x > rightArmPos.x && rightArmPos.x < rightForeArmPos.x
                            && rightShoulderPos.y > rightArmPos.y && rightArmPos.y > rightForeArmPos.y)
                    { text.text = "悪役"; Result = 1; }
                    else
                    { Result = 2; }
                    break;
                case 27:
                    if ((leftLegPos - rightLegPos).x > 25 && leftUpLegPos.x < leftLegPos.x
                        && rightUpLegPos.x > rightLegPos.x && leftShoulderPos.x < leftArmPos.x
                         && leftArmPos.x < leftForeArmPos.x && (leftShoulderPos - leftArmPos).y <= 10
                          && (leftShoulderPos - leftArmPos).y >= -10 && (leftArmPos - leftForeArmPos).y <= 10
                           && (leftArmPos - leftForeArmPos).y >= -10 && (rightShoulderPos - rightArmPos).x <= 10
                            && (rightShoulderPos).x >= -10 && (rightArmPos - rightForeArmPos).x <= 10
                             && (rightArmPos - rightForeArmPos).x >= -10 && rightShoulderPos.y < rightArmPos.y
                              && rightArmPos.y < rightForeArmPos.y)
                    { text.text = "交通整理"; Result = 1; }
                    else
                    { Result = 2; }

                    break;
                case 28://脚をクロスしてみる
                    if((leftLegPos-rightLegPos).x <= -5 && leftShoulderPos.y > leftArmPos.y
                        && leftArmPos.y > leftForeArmPos.y && rightShoulderPos.x > rightArmPos.x
                         && rightArmPos.x > rightForeArmPos.x && rightShoulderPos.y > rightArmPos.y
                          && rightArmPos.y > rightForeArmPos.y)
                    { text.text = "女性の決めポーズかな？"; Result = 1; }
                    else
                    { Result = 2; }

                    break;
                case 29:
                    if((leftLegPos-rightLegPos).x >=20 && leftUpLegPos.x < leftLegPos.x 
                        && leftUpLegPos.y > leftLegPos.y && rightUpLegPos.y > rightLegPos.y
                         && rightUpLegPos.x > rightLegPos.x && leftShoulderPos.x > leftArmPos.x
                          && leftArmPos.x > leftForeArmPos.x && leftShoulderPos.y < leftArmPos.y
                           && leftArmPos.y < leftForeArmPos.y && rightShoulderPos.x < leftArmPos.x 
                            && rightArmPos.x < rightForeArmPos.x && rightShoulderPos.y < rightArmPos.y
                             && rightArmPos.y < rightForeArmPos.y)
                    { text.text = "人のポーズ"; Result = 1; }
                    else
                    { Result = 2; }

                    break;
                case 30:
                    if (leftUpLegPos.x > leftLegPos.x && leftShoulderPos.x < leftArmPos.x
                        && leftArmPos.x < leftForeArmPos.x && leftShoulderPos.y > leftArmPos.y
                         && leftArmPos.y < leftForeArmPos.y && rightShoulderPos.x > rightArmPos.x
                          && rightArmPos.x > rightForeArmPos.x && rightShoulderPos.y > rightArmPos.y
                           && rightArmPos.y > rightForeArmPos.y)
                    { text.text = "案山子"; Result = 1; }
                    else
                    { Result = 2; }
                    break;
            }
        }
    }
}