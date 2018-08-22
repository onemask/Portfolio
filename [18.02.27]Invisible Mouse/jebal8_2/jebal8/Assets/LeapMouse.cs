using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Leap;
using System;

public class LeapMouse : MonoBehaviour
{
    //Leap을 사용하기 위해 필요한 변수
    Controller controller;
    Frame frame;

    bool indexPalmMove; //Palm(손바닥)모드로 사용하기 위한 변수
    bool indexMove; //Index(손가락)모드로 사용하기 위한 변수
    bool ILYgesture; //Palm모드 사용시 캘리브레이션을 하기 위한 변수
    bool cali_flag; //캘리브레이션 실행을 알려주는 변수

    //캘리브레이션에 사용되는 변수
    float palm_x_max;
    float palm_x_min;
    float palm_y_max;
    float palm_y_min;
    float palm_x_now;
    float palm_y_now;

    //인식영역에 알맞게 커서가 움직이기 위한 변수
    float x_ratio;
    float y_ratio;

    //Index모드 위치와 속도에 사용되는 변수
    float idx_speed;
    float idy_speed;
    float idx_ratio;
    float idy_ratio;

    int y_cali; //캘리브레이션 실행 여부

    int currentResolutionWidth; //현재 스크린 너비
    int currentResolutionHeight;  // 현재 스크린의 높이
    int currentResolutionCenterWidth;
    int currentResolutionCenterHeight;

    //이미지 블러오기 위한 객체
    GameObject rewind;
    GameObject modechange;
    GameObject twoFinger;
    GameObject Iclick;
    GameObject Pmove;
    GameObject Imove;
    GameObject ILY;
    GameObject def;
    GameObject palmUp;
    GameObject palmDown;
    GameObject speedUp;
    GameObject speedDown;
    GameObject fastForward;
    GameObject backward;
    GameObject forward;
    GameObject wheelControl;
    GameObject wheelControl2;
    GameObject wheelControl3;
    GameObject wheelControl4;

    // for mouse control
    [DllImport("user32")]
    static extern bool SetCursorPos(int X, int Y); //커서를 해당 위치로 설정
    [DllImport("User32")]
    static extern void mouse_event(ulong flag, int X, int Y, int dwData); //클릭과 같은 이벤트 발생 위한 함수

    //keyboard control
    [DllImport("User32")]
    static extern void keybd_event(uint vk, uint scan, uint flags, uint extraInfo); //키보드 입력을 위한 함수

    //좌클릭
    ulong MOUSEEVENTF_LEFTDOWN = 0x0002;
    ulong MOUSEEVENTF_LEFTUP = 0x0004;
    //우클릭
    ulong MOUSEEVENTF_RIGHTDOWN = 0x0008;
    ulong MOUSEEVENTF_RIGHTUP = 0x0010;
    //마우스휠
    ulong MOUSEEVENTF_WHEEL = 0x800;

    //Swipe동작에서 이전 조건 만족에 대한 기억을 위한 변수
    bool fingerSwipeR1;
    bool fingerSwipeR2;
    bool fingerSwipeL1;
    bool fingerSwipeL2;

    //뒤로가기 기능에서 이전 조건 만족에 대한 기억을 위한 변수
    bool backPage1;
    bool backPage2;
    bool forwardPage1;
    bool forwardPage2;

    bool dragAndDrop; // 마우스 왼쪽버튼을 누르고 떼는 동작을 정의하기 위한 변수

    //안녕 동작에서 이전 조건 만족에 대한 기억을 위한 변수
    bool inToOut;
    bool outToIn;

    bool doubleClick;

    void Start()
    {
        controller = new Controller();

        //화면 사이즈 조절
        Screen.SetResolution(240 * 16 / 9, 240, false);

        //현재 화면의 해상도
        currentResolutionWidth = Screen.currentResolution.width;  //해상도 x
        currentResolutionHeight = Screen.currentResolution.height;  // 해상도y
        currentResolutionCenterWidth = currentResolutionWidth / 2;
        currentResolutionCenterHeight = currentResolutionHeight / 2;

        //association cali
        palm_x_min = 0.0f;
        //palm_x_max = 0.0f;
        palm_y_max = 0.0f;
        palm_y_min = currentResolutionCenterHeight;

        //mouse moving value
        x_ratio = 4.0f;
        y_ratio = 3.0f;
        cali_flag = false;
        //y_cali = currentResolutionCenterHeight - 200;

        //swipe 동작 변수 초기화
        fingerSwipeR1 = false;
        fingerSwipeR2 = false;
        fingerSwipeL1 = false;
        fingerSwipeL2 = false;

        //뒤로가기 기능 변수 초기화
        backPage1 = false;
        backPage2 = false;
        forwardPage1 = false;
        forwardPage2 = false;

        dragAndDrop = false; //클릭이벤트 변수 초기화

        doubleClick = false; //Palm모드 더블클릭 조건

        //Making the img
        rewind = GameObject.Find("rewind") as GameObject;
        modechange = GameObject.Find("modeChange") as GameObject;
        twoFinger = GameObject.Find("twoFinger") as GameObject;
        Iclick = GameObject.Find("Iclick") as GameObject;
        Imove = GameObject.Find("Imove") as GameObject;
        Pmove = GameObject.Find("Pmove") as GameObject;
        ILY = GameObject.Find("ILY") as GameObject;
        def = GameObject.Find("def") as GameObject;
        palmUp = GameObject.Find("palmUp") as GameObject;
        palmDown = GameObject.Find("palmDown") as GameObject;
        speedUp = GameObject.Find("speedUp") as GameObject;
        speedDown = GameObject.Find("speedDown") as GameObject;
        fastForward = GameObject.Find("fastForward") as GameObject;
        backward = GameObject.Find("backward") as GameObject;
        forward = GameObject.Find("forward") as GameObject;
        wheelControl = GameObject.Find("wheelControl") as GameObject;
        wheelControl2 = GameObject.Find("wheelControl2") as GameObject;
        wheelControl3 = GameObject.Find("wheelControl3") as GameObject;
        wheelControl4 = GameObject.Find("wheelControl4") as GameObject;

        //Index모드 위치, 속도
        idx_speed = 0.0f;
        idy_speed = 0.0f;
        idx_ratio = 2.0f;
        idy_ratio = 2.0f;

        appear(def); //기본 이미지 출력

    }

    // Update is called once per frame
    void Update()
    {
        //frame에서 hands를 판별하기 위한 선언들
        controller = new Controller();
        Frame frame = controller.Frame();
        List<Hand> hands = frame.Hands;
        Hand firstHand = null; //첫번째 손 존재하는지
        Hand secondHand = null; //두번째 손 존재하는지

        if (frame.Hands.Count > 0)
        {
            firstHand = hands[0];

            if (frame.Hands.Count > 1)
            {
                secondHand = hands[1];
            }
        }
        else
        {
            appear(def); //손 입력 없을시 기본 이미지
        }

        Vector position = firstHand.PalmPosition; //손바닥 위치를 알아내기 위한 변수

        //각 손가락이 향하는 방향을 알기 위한 변수
        Vector thumbDir = firstHand.Fingers[0].Direction;
        Vector indexDir = firstHand.Fingers[1].Direction;
        Vector thirdDir = firstHand.Fingers[2].Direction;
        Vector fourthDir = firstHand.Fingers[3].Direction;
        Vector babyDir = firstHand.Fingers[4].Direction;

        Debug.Log(firstHand.PalmNormal.x);
        Debug.Log(firstHand.PalmNormal.y);
        Debug.Log(firstHand.PalmNormal.z);
        //두손일때 실행된다
        if (frame.Hands.Count == 2)
        {
            Hand left = null;
            Hand right = null;
            //먼저 들어온 손이 왼손일 경우 left에 첫번째 손을 right에 두번째 손을 초기화시켜준다
            if (firstHand.IsLeft == true)
            {
                left = firstHand;
                right = secondHand;
            }
            //먼저 들어온 손이 오른손일 경우 right에 첫번째 손을 left에 두번째 손을 초기화시켜준다
            else if (secondHand.IsLeft == true)
            {
                left = secondHand;
                right = firstHand;
            }

            //모드변경과 종료를 위한 조건
            //손이 오른쪽으로 이동하면 양수 왼쪽으로 이동하면 음수인데, 양손을 마주치는 상황을 위해서 왼손은 양수 오른손은 음수의 일정속도 이상이어야 한다.
            if (left.PalmVelocity.x > 400 && right.PalmVelocity.x < -400)
            {
                //양 손의 위치 차이를 확인하기 위해 x좌표를 사용한다
                //절대값은 원점이 아니더라도 박수를 인식하기 위함이고 일반값은 실제로 손이 가까워져야 함을 나타낸다.
                float firAbs = Math.Abs(left.PalmPosition.x);
                float secAbs = Math.Abs(right.PalmPosition.x);
                float fir = left.PalmPosition.x;
                float sec = right.PalmPosition.x;
                float palmDiffer = 0f;
                float palmDifferAbs = 0f;
                if (firAbs > secAbs)
                {
                    palmDifferAbs = firAbs - secAbs;
                }
                else
                {
                    palmDifferAbs = secAbs - firAbs;
                }
                if (fir > sec)
                {
                    palmDiffer = fir - sec;
                }
                else
                {
                    palmDiffer = sec - fir;
                }

                if (palmDifferAbs < 50 && palmDiffer < 50) //Leap이 안정적으로 인식하는 좁은범위를 설정
                {
                    //모드변경
                    //손을 편 상태에서 양 손을 마주보게 위치하는 조건 손이 너무 아래나 위쪽으로 향하면 안된다
                    if (left.GrabStrength < 0.1 && right.GrabStrength < 0.1 && left.PalmNormal.x > 0.8 && right.PalmNormal.x < -0.8 && left.PalmNormal.z < 0.3 && left.PalmNormal.z > -0.2 && right.PalmNormal.z < 0.3 && right.PalmNormal.z > -0.2)
                    {
                        speedReset(); //모드변경시 위치와 속도를 처음과 같이 초기화시켜준다
                        appear(modechange); //모드변경 이미지 출력
                        //현재 모드가 Index면 Palm모드로 변경
                        if (indexMove == true)
                        {
                            indexMove = false;
                            indexPalmMove = true;
                        }
                        //현재 모드가 Palm이면 Index모드로 변경
                        else if (indexPalmMove == true)
                        {
                            indexPalmMove = false;
                            indexMove = true;
                        }
                        System.Threading.Thread.Sleep(100); //여러번 실행되는 것을 방지하기 위해 약간의 딜레이 추가
                    }
                    //종료
                    //주먹 쥐고 박수치면 동작 성립
                    else if (left.GrabStrength == 1 && right.GrabStrength == 1)
                    {
                        Debug.Log("종료 ");
                        Application.Quit();
                    }
                }
            }

            //*******************************racing wheel 
            //위, 아래 이동을 위한 z좌표
            float left_z = left.PalmPosition.z;
            float right_z = right.PalmPosition.z;
            //좌우 이동을 위한 y좌표
            float left_y = left.PalmPosition.y;
            float right_y = right.PalmPosition.y;

            //좌우 이동을 위한 차이 식별 변수
            float lefty_righty = (left_y - right_y);
            float righty_lefty = (right_y - left_y);

            //손이 너무 가까우면 미인식, 레이싱 휠의 느낌을 위함
            float fistDiffer = right.PalmPosition.x - left.PalmPosition.x;

            //주먹을 쥐고 손의 위치가 일정 이하일때 
            if (firstHand.GrabStrength == 1 && secondHand.GrabStrength == 1 && 200 < fistDiffer)
            {
                if (-70 < left_z && left_z < 80 && -70 < right_z && right_z < 80)//상하 이동범위가 아닌 위치
                {
                    //Plain left right . 
                    if (left.PalmPosition.y > right.PalmPosition.y && lefty_righty > 80) //왼손의 y위치가 더 높고 차이가 80이상 날때
                    {
                        Debug.Log("Plain 오른쪽으로 이동 ");
                        appear(wheelControl2);
                        keybd_event(0x44, 0, 0x00, 0); //유투브에서 360도 영상은 wasd키로 이동이 가능하다. 오른쪽 이동은 D키를 사용한다
                        System.Threading.Thread.Sleep(70);
                        keybd_event(0x44, 0, 0x02, 0);
                        System.Threading.Thread.Sleep(70);
                    }
                    else if (left.PalmPosition.y < right.PalmPosition.y && righty_lefty > 80)//오른손의 y위치가 더 높고 차이가 80이상 날때
                    {
                        Debug.Log("Plain 왼쪽으로 이동 ");
                        appear(wheelControl);
                        keybd_event(0x41, 0, 0x00, 0); //유투브에서 360도 영상은 wasd키로 이동이 가능하다. 왼쪽 이동은 A키를 사용한다
                        System.Threading.Thread.Sleep(70);
                        keybd_event(0x41, 0, 0x02, 0);
                        System.Threading.Thread.Sleep(70);
                    }
                }
                else if (left_z < -70 && right_z < -70)//앞으로 손을 이동시키면
                {
                    //forward left right . 
                    Debug.Log("forward 이동 ");
                    appear(wheelControl3);
                    keybd_event(0x57, 0, 0x00, 0);
                    System.Threading.Thread.Sleep(70);
                    keybd_event(0x57, 0, 0x02, 0);  // W 키
                    System.Threading.Thread.Sleep(70);
                }

                else if (left_z > 90 && right_z > 90)//몸쪽으로 손을 이동시키면
                {
                    //backward left right . 
                    Debug.Log("backward 이동 ");
                    appear(wheelControl4);
                    keybd_event(0x53, 0, 0x00, 0);
                    System.Threading.Thread.Sleep(70);
                    keybd_event(0x53, 0, 0x02, 0);  // S 키
                    System.Threading.Thread.Sleep(70);
                }
            }
            //***********************Bye Bye motion

            //손바닥 법선벡터를 받는다
            Vector leftPalmNormal = left.PalmNormal;
            Vector rightPalmNormal = right.PalmNormal;

            //손바닥 속력
            float leftPalmSpeed = left.PalmVelocity.Magnitude;
            float rightPalmSpeed = right.PalmVelocity.Magnitude;

            //손바닥을 펴서 정면을 향해 들고 있는 동작
            if (left.PalmNormal.z < -0.5 && right.PalmNormal.z < -0.5 && left.GrabStrength < 1 && right.GrabStrength < 1)
            {
                //왼손 오른손 거리 차이
                float firDiffer = Math.Abs(left.Fingers[1].TipPosition.x);
                float secDiffer = Math.Abs(right.Fingers[1].TipPosition.x);
                float byeDiffer = 0f;

                if (firDiffer > secDiffer)
                {
                    byeDiffer = firDiffer - secDiffer;
                }
                else
                {
                    byeDiffer = secDiffer - firDiffer;
                }
                if (byeDiffer < 50 && leftPalmSpeed > 200 && rightPalmSpeed > 200) //양 손이 안쪽을 향하고 속도가 200이상
                {
                    inToOut = true;
                    if (inToOut == true || (left.PalmNormal.x < -0.25 && right.PalmNormal.x < -0.25 && right.Fingers[1].Direction.x < 0 && left.Fingers[1].Direction.x > 0 && byeDiffer > 100 && leftPalmSpeed > 200 && rightPalmSpeed > 200 && left.PalmNormal.y > -0.5 && right.PalmNormal.y > -0.5 && left.PalmNormal.z < -0.5 && right.PalmNormal.z < -0.5))

                    {
                        Debug.Log("안녕  ");
                        inToOut = false;
                    }
                }

            }

            //두번째 인식된 손이 주먹일때
            if (!secondHand.Fingers[0].IsExtended && !secondHand.Fingers[1].IsExtended && !secondHand.Fingers[2].IsExtended && !secondHand.Fingers[3].IsExtended && !secondHand.Fingers[4].IsExtended)
            {
                if (firstHand.PalmNormal.y < 0.4 && firstHand.PalmNormal.y > -0.4)//손바닥을 세워서 동작
                {
                    //되감기
                    //첫번째 손이 오른손이고 모두 편상태. 방향이 불확실한 엄지를 제외한 나머지 4손가락의 방향 측정한다. 손가락들의 방향이 오른쪽
                    if ((!firstHand.IsLeft && indexDir.x > 0.20 && thirdDir.x > 0.20 && fourthDir.x > 0.20 && babyDir.x > 0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || fingerSwipeL1 == true)
                    {
                        fingerSwipeL1 = true; //연속 동작을 판별하기 위한 상태 저장 변수
                                              //모두 펴져있고 손가락들의 방향이 z축과 수평한 위치보다 약간 왼쪽이다.
                        if ((indexDir.x < -0.20 && thirdDir.x < -0.20 && fourthDir.x < -0.20 && babyDir.x < -0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || fingerSwipeL2 == true)
                        {
                            fingerSwipeL2 = true; //연속 동작을 판별하기 위한 상태 저장 변수
                                                  //모두 펴져있고 손가락들의 방향이 확실히 왼쪽을 향하고 있다
                            if (indexDir.x < -0.5 && thirdDir.x < -0.5 && fourthDir.x < -0.5 && babyDir.x < -0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                            {
                                appear(rewind);
                                keybd_event(0x4A, 0, 0x00, 0); //유투브에서 10초 되감기는 J키로 가능하다
                                keybd_event(0x4A, 0, 0x02, 0);
                                fingerSwipeL1 = false;
                                fingerSwipeL2 = false;
                            }
                            //최종 조건에 도달하기 전 z축과 수평보다 커지면 상태저장 변수들을 해제한다.
                            else if (indexDir.x > 0 && thirdDir.x > 0 && fourthDir.x > 0 && babyDir.x > 0)
                            {
                                fingerSwipeL1 = false;
                                fingerSwipeL2 = false;
                            }
                        }
                    }

                    //빨리감기
                    //첫번째 손이 왼손이고 모두 편상태. 방향이 불확실한 엄지를 제외한 나머지 4손가락의 방향 측정한다. 손가락들의 방향이 왼쪽
                    else if ((firstHand.IsLeft && indexDir.x < -0.2 && thirdDir.x < -0.2 && fourthDir.x < -0.2 && babyDir.x < -0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || fingerSwipeR1 == true)
                    {
                        fingerSwipeR1 = true; //연속 동작을 판별하기 위한 상태 저장 변수
                                              //모두 펴져있고 손가락들의 방향이 z축과 수평한 위치보다 약간 오른쪽이다.
                        if ((indexDir.x > 0.20 && thirdDir.x > 0.20 && fourthDir.x > 0.20 && babyDir.x > 0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || fingerSwipeR2 == true)
                        {
                            fingerSwipeR2 = true; //연속 동작을 판별하기 위한 상태 저장 변수
                                                  //모두 펴져있고 손가락들의 방향이 확실히 오른쪽을 향하고 있다
                            if (indexDir.x > 0.5 && thirdDir.x > 0.5 && fourthDir.x > 0.5 && babyDir.x > 0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                            {
                                appear(fastForward);
                                keybd_event(0x4C, 0, 0x00, 0); //유투브에서 10초 빨리감기는 L키로 가능하다
                                keybd_event(0x4C, 0, 0x02, 0);
                                fingerSwipeR1 = false;
                                fingerSwipeR2 = false;
                            }
                            //최종 조건에 도달하기 전 z축과 수평보다 작아지면 상태저장 변수들을 해제한다.
                            else if (indexDir.x < 0 && thirdDir.x < 0 && fourthDir.x < 0 && babyDir.x < 0)
                            {
                                fingerSwipeR1 = false;
                                fingerSwipeR2 = false;
                            }
                        }
                    }
                    //위의 조건들이 아닐시 변수 초기화
                    else
                    {
                        fingerSwipeL1 = false;
                        fingerSwipeL2 = false;
                        fingerSwipeR1 = false;
                        fingerSwipeR2 = false;
                    }
                }
            }
        }

        //한손일때 실행된다
        if (frame.Hands.Count == 1)
        {

            float speed1 = firstHand.Fingers[1].TipVelocity.y; //검지의 y축 이동 속도
            float speed2 = firstHand.Fingers[2].TipVelocity.y; //중지의 y축 이동 속도
            Vector handSpeed = firstHand.PalmVelocity; //손바닥 이동 속도 측정

            //손바닥이 왼쪽 혹은 오른쪽을 보지 않고 아래쪽을 향하도록
            if (firstHand.PalmNormal.x < 0.4 && firstHand.PalmNormal.x > -0.4 && firstHand.PalmNormal.y < -0.3)
            {
                //indexMode 속도 제어
                //손바닥을 앞으로 내밀고 다섯손가락 모두 들어올린다. 다섯손가락 모두 편 상태
                if (position.z < -80 && thumbDir.y > 0.15 && indexDir.y > 0.5 && thirdDir.y > 0.5 && fourthDir.y > 0.5 && babyDir.y > 0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                {
                    appear(speedUp);
                    //일정속도 이상으로 올라가지 않도록 한다
                    if (idx_ratio < 8.0f && idy_ratio < 8.0f)
                    {
                        idx_ratio = idx_ratio + 0.5f;
                        idy_ratio = idy_ratio + 0.5f;
                        Debug.Log("up");
                        System.Threading.Thread.Sleep(400);//중복 인식을 막기위한 딜레이
                    }
                }
                //손을 앞으로 내밀고 다섯손가락 모두 내린다. 다섯손가락 모두 편 상태
                else if (position.z < -80 && thumbDir.y < -0.15 && indexDir.y < -0.5 && thirdDir.y < -0.5 && fourthDir.y < -0.5 && babyDir.y < -0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                {
                    appear(speedDown);
                    //일정속도 이하로 내려가지 않도록 한다
                    if (idx_ratio > 0.8f && idy_ratio > 0.8f)
                    {
                        idx_ratio = idx_ratio - 0.5f;
                        idy_ratio = idy_ratio - 0.5f;
                        Debug.Log("down");
                        System.Threading.Thread.Sleep(400);
                    }
                }
                //scroll 제어
                //다섯손가락 모두 들어올린다. 다섯손가락 모두 편 상태
                else if (thumbDir.y > 0.15 && indexDir.y > 0.5 && thirdDir.y > 0.5 && fourthDir.y > 0.5 && babyDir.y > 0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                {
                    appear(palmUp);
                    //이동중 스크롤 업다운시 클릭 이벤트와 겹침이 발생
                    mouse_event(MOUSEEVENTF_LEFTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                    //현재 위치에서 스크롤 up
                    mouse_event(MOUSEEVENTF_WHEEL, 0, 75, 0);
                    //mouse_event(MOUSEEVENTF_WHEEL, 100,100, -60);
                }
                //다섯손가락 모두 내린다. 다섯손가락 모두 편 상태
                else if (thumbDir.y < -0.15 && indexDir.y < -0.5 && thirdDir.y < -0.5 && fourthDir.y < -0.5 && babyDir.y < -0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                {
                    appear(palmDown);
                    //이동중 스크롤 업다운시 클릭 이벤트와 겹침이 발생
                    mouse_event(MOUSEEVENTF_LEFTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                    //현재 위치에서 스크롤 down
                    mouse_event(MOUSEEVENTF_WHEEL, 0, -75, 0);
                    //mouse_event(MOUSEEVENTF_WHEEL, 100,100, -60);
                }

            }


            //뒤로가기 앞으로가기
            if(firstHand.PalmNormal.y < 0.4 && firstHand.PalmNormal.y > -0.4)//손바닥을 세워서 동작
            {
                //첫번째 손이 오른손이고 모두 편상태. 방향이 불확실한 엄지를 제외한 나머지 4손가락의 방향 측정한다. 손가락들의 방향이 오른쪽
                if ((!firstHand.IsLeft && indexDir.x > 0.20 && thirdDir.x > 0.20 && fourthDir.x > 0.20 && babyDir.x > 0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || backPage1 == true)
                {
                    backPage1 = true;//연속 동작을 판별하기 위한 상태 저장 변수
                                     //모두 펴져있고 손가락들의 방향이 z축과 수평한 위치보다 약간 왼쪽이다.
                    if ((indexDir.x < -0.20 && thirdDir.x < -0.20 && fourthDir.x < -0.20 && babyDir.x < -0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || backPage2 == true)
                    {
                        backPage2 = true;
                        //모두 펴져있고 손가락들의 방향이 확실히 왼쪽을 향하고 있다
                        if (indexDir.x < -0.5 && thirdDir.x < -0.5 && fourthDir.x < -0.5 && babyDir.x < -0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                        {
                            appear(backward);
                            //웹상에서 뒤로가기는 Alt + <-다
                            //Alt를 누른상태에서 방향키를 입력하고 뗀다.
                            keybd_event(0x12, 0, 0x00, 0);
                            keybd_event(0x25, 0, 0x00, 0);
                            keybd_event(0x25, 0, 0x02, 0);
                            keybd_event(0x12, 0, 0x02, 0);
                            backPage1 = false;
                            backPage2 = false;
                        }
                        //최종 조건에 도달하기 전 z축과 수평보다 커지면 상태저장 변수들을 해제한다.
                        else if (indexDir.x > 0 && thirdDir.x > 0 && fourthDir.x > 0 && babyDir.x > 0)
                        {
                            backPage1 = false;
                            backPage2 = false;
                        }
                    }
                }

                //첫번째 손이 왼손이고 모두 편상태. 방향이 불확실한 엄지를 제외한 나머지 4손가락의 방향 측정한다. 손가락들의 방향이 왼쪽
                else if ((firstHand.IsLeft && indexDir.x < -0.2 && thirdDir.x < -0.2 && fourthDir.x < -0.2 && babyDir.x < -0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || forwardPage1 == true)
                {
                    forwardPage1 = true;
                    //모두 펴져있고 손가락들의 방향이 z축과 수평한 위치보다 약간 오른쪽이다.
                    if ((indexDir.x > 0.20 && thirdDir.x > 0.20 && fourthDir.x > 0.20 && babyDir.x > 0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || forwardPage2 == true)
                    {
                        forwardPage2 = true;
                        //모두 펴져있고 손가락들의 방향이 확실히 오른쪽을 향하고 있다
                        if (indexDir.x > 0.5 && thirdDir.x > 0.5 && fourthDir.x > 0.5 && babyDir.x > 0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                        {
                            appear(forward);
                            //웹상에서 뒤로가기는 Alt + ->다
                            //Alt를 누른상태에서 방향키를 입력하고 뗀다.
                            keybd_event(0x12, 0, 0x00, 0);
                            keybd_event(0x27, 0, 0x00, 0);
                            keybd_event(0x27, 0, 0x02, 0);
                            keybd_event(0x12, 0, 0x02, 0);
                            forwardPage1 = false;
                            forwardPage2 = false;
                        }
                        //최종 조건에 도달하기 전 z축과 수평보다 작아지면 상태저장 변수들을 해제한다.
                        else if (indexDir.x < 0 && thirdDir.x < 0 && fourthDir.x < 0 && babyDir.x < 0)
                        {
                            forwardPage1 = false;
                            forwardPage2 = false;
                        }
                    }
                }
                else
                {
                    backPage1 = false;
                    backPage2 = false;
                    forwardPage1 = false;
                    forwardPage2 = false;
                }
            }

            //index move start
            //검지는 펴고 나머지 손가락들은 주먹쥔 상태
            if (!firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended)
            {
                //기본 모드는 indexMove 모드가 된다.
                if (indexPalmMove == false)
                {
                    indexMove = true;
                }
                //모드 변경으로 바뀐다.
                else if (indexMove == false)
                {
                    indexPalmMove = true;
                }
            }
            //엄지,검지,새끼손가락은 펴고 중지, 약지를 접은 상태
            else if (firstHand.Fingers[4].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended)
            {
                //캘리브레이션을 실행하도록 변수를 true로 초기화
                ILYgesture = true;
            }
            //조건에 부합하지 않을때
            else
            {
                y_cali = currentResolutionCenterHeight - 200; //캘리브레이션 이전 y좌표 보정
                ILYgesture = false;
            }

            //캘리브레이션 동작이 실행되었고 현재 이동 모드가 Palm일때
            if (ILYgesture == true && indexPalmMove == true)
            {
                appear(ILY);
                //현재 x와 y좌표를 입력받고 이전에 갱신해오도 min, max값과 비교해 작거나 크면 적절하게 변경한다.
                palm_x_now = position.x;
                if (palm_x_max < palm_x_now)
                {   // 최대< 현재
                    palm_x_max = palm_x_now;    //최대 <- 현재
                }
                else if (palm_x_min > palm_x_now)
                {
                    palm_x_min = palm_x_now;
                }
                // y range calculation
                palm_y_now = position.y;
                if (palm_y_max < palm_y_now)
                {
                    palm_y_max = palm_y_now;
                }
                else if (palm_y_min > palm_y_now)
                {
                    palm_y_min = palm_y_now;
                }

                // x, y range, x와 y의 최대,최소값의 차이
                float x_leap = palm_x_max - palm_x_min;
                float y_leap = palm_y_max - palm_y_min;

                // x, y ratio 현재 해상도를 차이값으로 나눠 이동비율을 결정한다.
                x_ratio = currentResolutionWidth / x_leap;
                y_ratio = currentResolutionHeight / y_leap;

                cali_flag = true;
            }

            //Palm Mode Control.
            //Palm 모드면서 중지,약지,새끼손가락은 접은 상태
            else if (indexPalmMove == true && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended)
            {
                if (cali_flag == true)
                {
                    y_cali = currentResolutionCenterHeight + 200; //캘리브레이션 이후 캘리브레이션 보정
                }

                if (firstHand.Fingers[1].IsExtended)
                {
                    appear(Pmove);
                    SetCursorPos(currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y)); // this!
                    doubleClick = true; //한번의 더블클릭을 위한 변수
                }

                //z축 앞으로 많이 나가게 되면 검지가 펴지는 현상이 있는데 이 현상으로 인한 움직임 방지
                if (firstHand.PalmPosition.z > -85)
                {
                    //검지 편 상태
                    if (firstHand.Fingers[1].IsExtended)
                    {
                        //엄지를 편상태에서 접으면 클릭버튼을 뗀다
                        if (dragAndDrop == true && !firstHand.Fingers[0].IsExtended)
                        {
                            //좌클릭 뗌
                            mouse_event(MOUSEEVENTF_LEFTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                            System.Threading.Thread.Sleep(100);
                            Debug.Log(" 드래그  떼어짐. . ");
                            dragAndDrop = false;
                        }
                        //검지만 편 상태에서 엄지를 펴면 클릭버튼 누른다. Leap모션의 하드웨어적 오류보정을 위해서 엄지 발견 후 0.4초 이후 부터 가능하며 속력이 600이하여야 한다.
                        else if (firstHand.Fingers[0].IsExtended && dragAndDrop == false && firstHand.Fingers[0].TimeVisible > 40000 && firstHand.PalmVelocity.Magnitude < 600)
                        {
                            appear(Iclick);
                            //좌클릭 누름
                            mouse_event(MOUSEEVENTF_LEFTDOWN, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                            System.Threading.Thread.Sleep(100);
                            dragAndDrop = true;
                            Debug.Log(" 드래그  눌림. ");
                        }
                    }
                    //검지 접은 상태
                    else
                    {
                        //Palm모드에서 불안정한 클릭때문에 추가한 더블클릭, 검지만 핀채 이동중 검지를 접으면 동작한다.
                        if (!firstHand.Fingers[0].IsExtended && doubleClick)
                        {
                            mouse_event(MOUSEEVENTF_LEFTDOWN, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                            System.Threading.Thread.Sleep(100);
                            mouse_event(MOUSEEVENTF_LEFTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                            System.Threading.Thread.Sleep(100);
                            mouse_event(MOUSEEVENTF_LEFTDOWN, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                            System.Threading.Thread.Sleep(100);
                            mouse_event(MOUSEEVENTF_LEFTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                            System.Threading.Thread.Sleep(100);
                            doubleClick = false;
                        }
                    }
                }
            }


            //IDXMODE
            //Index 모드면서 검지는 펴고 중지,약지,새끼손가락은 접은 상태
            else if (indexMove && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended) //여기다 넣어요
            {
                //안정적인 클릭을 위해 속도 튀는 것을 방지하기 위해 속도 조건
                if (firstHand.Fingers[1].TipVelocity.Magnitude < 600)
                {
                    appear(def);
                    SetCursorPos(currentResolutionCenterWidth + (int)xSetting(firstHand), currentResolutionCenterHeight + (int)ySetting(firstHand));
                }
                //z축 앞으로 많이 나가게 되면 검지가 펴지는 현상이 있는데 이 현상으로 인한 움직임 방지
                if (firstHand.PalmPosition.z > -85)
                {
                    //검지를 편 상태, 엄지를 편상태에서 접으면 클릭버튼을 뗀다
                    if (dragAndDrop == true && !firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended)
                    {
                        mouse_event(MOUSEEVENTF_LEFTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                        System.Threading.Thread.Sleep(100);
                        Debug.Log(" 드래그  떼어짐. ");
                        dragAndDrop = false;
                    }
                     //검지, 엄지를 펴면 클릭버튼 누른다. 나머지 손가락은 접힌상태. Leap모션의 하드웨어적 오류보정을 위해서 엄지 발견 후 0.4초 이후 부터 가능
                    else if (firstHand.Fingers[1].IsExtended && firstHand.Fingers[0].IsExtended && dragAndDrop == false && firstHand.Fingers[0].TimeVisible > 40000 && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended)
                    {
                        appear(Iclick);
                        mouse_event(MOUSEEVENTF_LEFTDOWN, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                        System.Threading.Thread.Sleep(100);
                        dragAndDrop = true;
                        Debug.Log(" 드래그  눌림. ");
                    }
                }
            }

            //우클릭 .
            //검지와 중지만 펴고 나머지는 접은상태에서 검지와 중지의 속도가 일정 이상이 되고 손바닥의 속력이 일정 이하일때 동작한다
            if ((!firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended) && speed2 < -400 && speed1 < -400 && handSpeed[0] < 150 && handSpeed[1] < 150 && handSpeed[2] < 150 && handSpeed[0] > -150 && handSpeed[1] > -150 && handSpeed[2] > -150)
            {
                appear(twoFinger);
                Debug.Log("우클릭");
                //현재 위치를 받아와서 마우스 우클릭을 한다.
                mouse_event(MOUSEEVENTF_RIGHTDOWN, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                System.Threading.Thread.Sleep(100);
                mouse_event(MOUSEEVENTF_RIGHTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                System.Threading.Thread.Sleep(150);
            }
        }
    }
    //모드 변경시 속도가 튀는 것을 방지하기 위해 값을 초기화 시켜주는 함수
    public void speedReset()
    {
        //palm모드 변수 초기화
        palm_x_min = 0.0f;
        //palm_x_max = 0.0f;
        palm_y_max = 0.0f;
        palm_y_min = currentResolutionCenterHeight;

        x_ratio = 4.0f;
        y_ratio = 3.0f;
        cali_flag = false;

        //index모드 변수 초기화
        idx_speed = 0.0f;
        idy_speed = 0.0f;

        idx_ratio = 2.0f;
        idy_ratio = 2.0f;
    }

    //index모드에서 x좌표의 이동을 담당하는 함수
    public float xSetting(Hand hand)
    {
        //손가락 끝의 방향이 왼쪽으로 움직이면 값이 작아져 마우스 커서가 왼쪽으로 움직인다
        if (hand.Fingers[1].Direction.x < -0.2) //&& idx_speed >= -currentResolutionCenterWidth)
        {
            //감소
            appear(Imove);
            idx_speed = idx_speed - (1 * idx_ratio);
        }
        else if (hand.Fingers[1].Direction.x > 0.2) //&& idx_speed <= currentResolutionCenterWidth)
        {
            //증가
            appear(Imove);
            idx_speed = idx_speed + (1 * idx_ratio);
        }
        return idx_speed;
    }

    //index모드에서 y좌표의 이동을 담당하는 함수
    public float ySetting(Hand hand)
    {
        //손가락 끝의 방향이 왼쪽으로 움직이면 값이 작아져 마우스 커서가 왼쪽으로 움직인다
        if (hand.Fingers[1].Direction.y < -0.25 && idy_speed <= currentResolutionCenterHeight)
        {
            appear(Imove);
            idy_speed = idy_speed + (1 * idy_ratio);
        }
        else if (hand.Fingers[1].Direction.y > 0.35 && idy_speed >= -currentResolutionCenterHeight)
        {
            appear(Imove);
            idy_speed = idy_speed - (1 * idy_ratio);
        }
        return idy_speed;
    }

    //어떤 객체를 화면상에 띄울것인지 결정하는 함수
    public void appear(GameObject obj)
    {
        //원하는 이미지만을 카메라에서 보기위해 다른 이미지들은 카메라 뒤쪽에 위치시킨다
        rewind.transform.position = new Vector3(0f, 0f, 2.0f);
        modechange.transform.position = new Vector3(0f, 0f, 2.0f);
        twoFinger.transform.position = new Vector3(0f, 0f, 2.0f);
        Iclick.transform.position = new Vector3(0f, 0f, 2.0f);
        Imove.transform.position = new Vector3(0f, 0f, 2.0f);
        Pmove.transform.position = new Vector3(0f, 0f, 2.0f);
        ILY.transform.position = new Vector3(0f, 0f, 2.0f);
        def.transform.position = new Vector3(0f, 0f, 2.0f);
        palmUp.transform.position = new Vector3(0f, 0f, 2.0f);
        palmDown.transform.position = new Vector3(0f, 0f, 2.0f);
        speedUp.transform.position = new Vector3(0f, 0f, 2.0f);
        speedDown.transform.position = new Vector3(0f, 0f, 2.0f);
        fastForward.transform.position = new Vector3(0f, 0f, 2.0f);
        backward.transform.position = new Vector3(0f, 0f, 2.0f);
        forward.transform.position = new Vector3(0f, 0f, 2.0f);
        wheelControl.transform.position = new Vector3(0f, 0f, 2.0f);
        wheelControl2.transform.position = new Vector3(0f, 0f, 2.0f);
        wheelControl3.transform.position = new Vector3(0f, 0f, 2.0f);
        wheelControl4.transform.position = new Vector3(0f, 0f, 2.0f);
        //매개변수로 들어오는 객체의 이미지만 출력한다.
        obj.transform.position = new Vector3(0f, 0f, 0.005f);
    }

}
