using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class KinectAvatar2 : MonoBehaviour {

    [SerializeField]
    Camera cam;

    [SerializeField]
    Vector3 leftLegPos;
    [SerializeField]
    Vector3 leftUpLegPos;

    [SerializeField]
    Vector3 rightLegPos;
    [SerializeField]
    Vector3 rightUpLegPos;

    [SerializeField]
    Vector3 spine1Pos;
    [SerializeField]
    Vector3 spine2Pos;

    [SerializeField]
    Vector3 hipsPos;
    [SerializeField]
    Vector3 leftHipPos;
    [SerializeField]
    Vector3 rightHipPos;

    [SerializeField]
    Vector3 leftShoulderPos;
    [SerializeField]
    Vector3 rightShoulderPos;

    [SerializeField]
    Vector3 leftArmPos;
    [SerializeField]
    Vector3 leftForeArmPos;
    [SerializeField]
    Vector3 leftHandPos;

    [SerializeField]
    Vector3 rightArmPos;
    [SerializeField]
    Vector3 rightForeArmPos;
    [SerializeField]
    Vector3 rightHandPos;


    //テキスト表示するため
    public Text text;

    //鏡にするかそのままにするかの判定
    public bool IsMirror = true;
    public int[] Poze=new int[4];//糞配列
    public int chack = 0;//どのポーズが出来ていないか
    public bool Result = true;//ポーズがちゃんと出来ている
    public int PozeCount=0;
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


    //最初に読み込む必要のあるものを表記
    void Start()
    {
        //ポーズの初期値を設定
        for (int i = 0; i < 4; i++)//ここの　i　<　○　の数値は登録してあるポーズにする
        {
            Poze[i] = i;
        }
        //ここでモデルをベースに各関節のGameObjを取得
        //アサインすることとする
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
        Quaternion HipRight;//爆弾投下なり
        Quaternion HipLeft;//馬鹿が馬鹿なりに追加してみた
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
            HipRight= joints[JointType.HipRight].Orientation.ToMirror().ToQuaternion(comp);//起爆
            HipLeft= joints[JointType.HipLeft].Orientation.ToMirror().ToQuaternion(comp);//成功してくれー（懇願
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
            HipRight = joints[JointType.HipLeft].Orientation.ToQuaternion(comp);//起爆
            HipLeft = joints[JointType.HipRight].Orientation.ToQuaternion(comp);//成功してくれー（懇願
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
        RightHips.transform.rotation = HipRight * comp2;
        RightUpLeg.transform.rotation = KneeRight * comp2;
        RightLeg.transform.rotation = AnkleRight * comp2;
        //左脚
        LeftHips.transform.rotation = HipLeft * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
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
            leftHipPos= cam.WorldToScreenPoint(LeftHips.transform.position);//爆弾　解放厳禁
            rightHipPos= cam.WorldToScreenPoint(RightHips.transform.position);
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
            if ((leftLegPos - rightLegPos).x > 20)
            {
                //脚は一定範囲開いていた //次に上半身の判定に移る
                //ここはのちのちポーズが増えることから細かく判定していく
                if (leftForeArmPos.x > leftHandPos.x && leftForeArmPos.y > leftHandPos.y
                    && leftShoulderPos.y > leftForeArmPos.y && leftForeArmPos.x > leftShoulderPos.x)
                {
                    //ここは左腕が＞の字になってるポーズ
                    if (rightForeArmPos.x < rightHandPos.x && rightForeArmPos.y > rightHandPos.y
                    && rightForeArmPos.x < rightShoulderPos.x && rightShoulderPos.y > rightForeArmPos.y)
                    {
                        //ヒーローの判定完成のはずであります
                        PozeCount = 1;
                    }
                }
                //大の字//これは100%一致のポーズは難しいためある程度の誤差を作っている
                else if ((leftHandPos - leftForeArmPos).y < 10 && (leftHandPos - leftForeArmPos).y < 10
                    && (leftShoulderPos - leftForeArmPos).y < 10 && (leftHandPos - leftForeArmPos).y < 10)
                {
                    //左手の判定に成功
                    if ((rightHandPos - rightForeArmPos).y < 10 && (rightHandPos - rightForeArmPos).y < 10
                    && (rightShoulderPos - rightForeArmPos).y < 10 && (rightHandPos - rightForeArmPos).y < 10)
                    {
                        //大の字に成功しました
                        PozeCount = 2;
                    }
                }
            }
            else if ((leftLegPos - rightLegPos).x <= 20)//脚を開いていない状態とする
            {
                //判定に成功しました
                //これから左手の判定をとりやがるです
                if (leftShoulderPos.x < leftHandPos.x && leftHandPos.y > leftForeArmPos.y
                    && leftShoulderPos.y < leftHandPos.y && leftForeArmPos.x < leftHandPos.x)
                {
                    if (rightShoulderPos.x > rightHandPos.x && rightHandPos.y > rightForeArmPos.y
                    && rightShoulderPos.y < rightHandPos.y && rightForeArmPos.x > rightHandPos.x)
                    {
                        //Y字の判定完成したです
                        PozeCount = 3;
                    }
                }
            }//ここで判定タイミングおわり

            //デバック用UI表示　判定が成功していれば表示される
            switch (Poze[PozeCount])
            {
                case 0://今現在判定していないものの可能性あり
                    text.text = "ポーズが読み込めません";
                    Result = false;

                    break;
                case 1://ヒーローらしい？
                    text.text = "ヒーロー参上デス、、";
                    Result = true;

                    break;
                case 2://大文字
                    text.text = "大の字成功でやんす";
                    Result = true;

                    break;
                case 3://Y字　よくわかりませんね
                    text.text = "ロムルスーーーーーーーーー";
                    Result = true;

                    break;
            }
        }
    }
}