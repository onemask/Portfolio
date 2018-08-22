  
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Leap;
using Leap.Unity;
using System;

public class LeapMouse : MonoBehaviour
{
    Controller controller;
    Frame frame;

    bool indexPalmMove;
    bool indexMove;
    bool ILYgesture;
    bool cali_flag;

    // for calibration
    float palm_x_max;
    float palm_x_min;

    float palm_y_max;
    float palm_y_min;

    float palm_x_now;
    float palm_y_now;

    float x_ratio;
    float y_ratio;
    float idx_speed;
    float idy_speed;
    float idx_ratio;
    float idy_ratio;

    int y_cali;

    int currentResolutionWidth; //현재 스크린 너비
    int currentResolutionHeight;  // 현재 스크린의 높이
    int currentResolutionCenterWidth;
    int currentResolutionCenterHeight;
    
    //Bring object Img
    GameObject rewind;
    GameObject modechange;
    GameObject threeFinger;
    GameObject twoFinger;
    GameObject Iclick;
    GameObject Pclick;
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
    GameObject drag;

    // for mouse control
    [DllImport("user32")]
    static extern bool SetCursorPos(int X, int Y);  //현재 커서 가지고 오는 거.
    [DllImport("User32")]
    static extern void mouse_event(ulong flag, int X, int Y, int dwData); //mouse로 event주는애.

    //keyboard control
    [DllImport("User32")]
    static extern void keybd_event(uint vk, uint scan, uint flags, uint extraInfo);

    //Build size setting
    [DllImport("user32.dll")]
    private static extern int GetActiveWindow();
    [DllImport("user32.dll")]
    private static extern long GetWindowLong(int hwnd, int nIndex);
    [DllImport("user32.dll")]
    static extern int SetWindowLong(int hwnd, int nIndex, long dwNewLong);
    [DllImport("user32.dll")]
    public static extern bool SetWindowText(int hwnd, System.String lpString);

    //좌클릭
    ulong MOUSEEVENTF_LEFTDOWN = 0x0002;
    ulong MOUSEEVENTF_LEFTUP = 0x0004;
    //우클릭
    ulong MOUSEEVENTF_RIGHTDOWN = 0x0008;
    ulong MOUSEEVENTF_RIGHTUP = 0x0010;
    //마우스휠
    ulong MOUSEEVENTF_WHEEL = 0x800;

    //rotate 변수
    bool fingerSwipeR1;
    bool fingerSwipeR2;

    bool fingerSwipeL1;
    bool fingerSwipeL2;

    bool backPage1;
    bool backPage2;
    bool forwardPage1;
    bool forwardPage2;

    bool dragAndDrop;
    
    void Start()
    {
        controller = new Controller();

        //Screen size setting
        Screen.SetResolution(240 * 16 / 9, 240, false);

        //initialization x,y
        currentResolutionWidth = Screen.currentResolution.width;  //해상도 x
        currentResolutionHeight = Screen.currentResolution.height;  // 해상도y
        currentResolutionCenterWidth = currentResolutionWidth / 2;
        currentResolutionCenterHeight = currentResolutionHeight / 2;

        //association cali
        palm_x_min = 0.0f;
        palm_y_max = 0.0f;
        palm_y_min = currentResolutionCenterHeight;

        //mouse moving value
        x_ratio = 4.0f;
        y_ratio = 3.0f;
        cali_flag = false;
        y_cali = currentResolutionCenterHeight - 200;

        //rotate
        fingerSwipeR1 = false;
        fingerSwipeR2 = false;

        fingerSwipeL1 = false;
        fingerSwipeL2 = false;

        backPage1 = false;
        backPage2 = false;
        forwardPage1 = false;
        forwardPage2 = false;

        dragAndDrop = false;

        //Making the img
        rewind = GameObject.Find("rewind") as GameObject;
        modechange = GameObject.Find("modeChange") as GameObject;
        twoFinger = GameObject.Find("twoFinger") as GameObject;
        threeFinger = GameObject.Find("threeFinger") as GameObject;
        Iclick = GameObject.Find("Iclick") as GameObject;
        Pclick = GameObject.Find("Pclick") as GameObject;
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
        drag = GameObject.Find("drag") as GameObject;
        

        idx_speed = 0.0f;
        idy_speed = 0.0f;
        
        idx_ratio = 2.0f;
        idy_ratio = 2.0f;

        appear(def);

    }

    // Update is called once per frame
    void Update()
    {
        controller = new Controller();
        Frame frame = controller.Frame();
        List<Hand> hands = frame.Hands;
        Hand firstHand = null;
        Hand secondHand = null;

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
            appear(def);
        }

        Vector position = firstHand.PalmPosition;

        Vector thumbDir = firstHand.Fingers[0].Direction;
        Vector indexDir = firstHand.Fingers[1].Direction;
        Vector thirdDir = firstHand.Fingers[2].Direction;
        Vector fourthDir = firstHand.Fingers[3].Direction;
        Vector babyDir = firstHand.Fingers[4].Direction;
        


        //application quit && mode change
        if (frame.Hands.Count == 2)
        {
            if (firstHand.IsLeft == true)
            {
                if (firstHand.PalmVelocity.x > 500 && secondHand.PalmVelocity.x < -500)
                {
                    float firAbs = Math.Abs(firstHand.PalmPosition.x); //첫번째 손 
                    float secAbs = Math.Abs(secondHand.PalmPosition.x); //두번째 손 
                    float differ = 0f;
                    if (firAbs > secAbs)
                    {
                        differ = firAbs - secAbs;
                    }
                    else
                    {
                        differ = secAbs - firAbs;
                    }

                    if (differ < 75)
                    {
                        if (firstHand.GrabStrength < 0.1 && secondHand.GrabStrength < 0.1 && firstHand.PalmNormal.x > 0.8 && secondHand.PalmNormal.x < -0.8)
                        {
                            speedReset();
                            appear(modechange);
                            if (indexMove == true)
                            {
                                indexMove = false;
                                indexPalmMove = true;
                            }
                            else if (indexPalmMove == true)
                            {
                                indexPalmMove = false;
                                indexMove = true;
                            }
                            System.Threading.Thread.Sleep(150);
                        }
                        else if (firstHand.GrabStrength == 1 && secondHand.GrabStrength == 1)
                        {
                            Debug.Log("종료 ");
                            Application.Quit();
                        }

                    }
                }
            }

            else //first hand is right hand. 
            {
                if (firstHand.PalmVelocity.x < -500 && secondHand.PalmVelocity.x > 500)
                {
                    float firAbs = Math.Abs(firstHand.PalmPosition.x);
                    float secAbs = Math.Abs(secondHand.PalmPosition.x);
                    float differ = 0f;
                    if (firAbs > secAbs)
                    {
                        differ = firAbs - secAbs;
                    }
                    else
                    {
                        differ = secAbs - firAbs;
                    }

                    if (differ < 75)
                    {
                        if (firstHand.GrabStrength < 0.1 && secondHand.GrabStrength < 0.1 && firstHand.PalmNormal.x < -0.8 && secondHand.PalmNormal.x > 0.8)
                        {
                            speedReset();
                            appear(modechange);

                            if (indexMove == true)
                            {
                                indexMove = false;
                                indexPalmMove = true;
                            }
                            else if (indexPalmMove == true)
                            {
                                indexPalmMove = false;
                                indexMove = true;
                            }
                            System.Threading.Thread.Sleep(150);
                        }
                        else if (firstHand.GrabStrength == 1 && secondHand.GrabStrength == 1)
                        {
                            Application.Quit();
                        }
                    }
                }
            }

            //only one hand 
            if (!secondHand.Fingers[0].IsExtended && !secondHand.Fingers[1].IsExtended && !secondHand.Fingers[2].IsExtended && !secondHand.Fingers[3].IsExtended && !secondHand.Fingers[4].IsExtended)
            {
                //rewind fastForward , 
                if ((!firstHand.IsLeft && indexDir.x > 0.20 && thirdDir.x > 0.20 && fourthDir.x > 0.20 && babyDir.x > 0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || fingerSwipeL1 == true)
                {
                    //Debug.Log("1");
                    fingerSwipeL1 = true;
                    if ((indexDir.x < -0.20 && thirdDir.x < -0.20 && fourthDir.x < -0.20 && babyDir.x < -0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || fingerSwipeL2 == true)
                    {
                        //Debug.Log("2");
                        fingerSwipeL2 = true;
                        if (indexDir.x < -0.5 && thirdDir.x < -0.5 && fourthDir.x < -0.5 && babyDir.x < -0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                        {
                            //Debug.Log("3");
                            appear(rewind);
                            keybd_event(0x4A, 0, 0x00, 0);
                            keybd_event(0x4A, 0, 0x02, 0);
                            fingerSwipeL1 = false;
                            fingerSwipeL2 = false;
                        }
                        else if (indexDir.x > 0 && thirdDir.x > 0 && fourthDir.x > 0 && babyDir.x > 0)
                        {
                            fingerSwipeL1 = false;
                            fingerSwipeL2 = false;
                            fingerSwipeR1 = false;
                            fingerSwipeR2 = false;
                        }
                    }

                }

                else if ((firstHand.IsLeft && indexDir.x < -0.2 && thirdDir.x < -0.2 && fourthDir.x < -0.2 && babyDir.x < -0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || fingerSwipeR1 == true)
                {
                    fingerSwipeR1 = true;
                    //Debug.Log("4");
                    if ((indexDir.x > 0.20 && thirdDir.x > 0.20 && fourthDir.x > 0.20 && babyDir.x > 0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || fingerSwipeR2 == true)
                    {
                        fingerSwipeR2 = true;
                        //Debug.Log("5");
                        if (indexDir.x > 0.5 && thirdDir.x > 0.5 && fourthDir.x > 0.5 && babyDir.x > 0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                        {
                            //Debug.Log("6");
                            appear(fastForward);
                            keybd_event(0x4C, 0, 0x00, 0);
                            keybd_event(0x4C, 0, 0x02, 0);
                            fingerSwipeR1 = false;
                            fingerSwipeR2 = false;
                        }
                        else if (indexDir.x < 0 && thirdDir.x < 0 && fourthDir.x < 0 && babyDir.x < 0)
                        {
                            fingerSwipeL1 = false;
                            fingerSwipeL2 = false;
                            fingerSwipeR1 = false;
                            fingerSwipeR2 = false;
                        }
                    }

                }
                else
                {
                    fingerSwipeL1 = false;
                    fingerSwipeL2 = false;
                    fingerSwipeR1 = false;
                    fingerSwipeR2 = false;
                }
            }
            //*******************************racing wheel 

            Hand left = null;
            Hand right = null;

            if (left == null && right == null)
            {
                float left_z = 0f;
                float right_z = 0f;

                float left_y = 0f;
                float right_y = 0f;

                float lefty_righty = (left_y - right_y);
                float righty_lefty = (right_y - left_y);
                float differ = 0f;


                //왼손이 먼저 들어올떄
                if (firstHand.IsLeft == true)
                {
                    left = firstHand;
                    right = secondHand;

                    left_z = left.PalmPosition.z;
                    right_z = right.PalmPosition.z;
                    left_y = left.PalmPosition.y;
                    right_y = right.PalmPosition.y;

                    differ = right.PalmPosition.x - left.PalmPosition.x;

                    //Debug.Log(secondHand.PalmPosition.x - firstHand.PalmPosition.x);

                    if (firstHand.GrabStrength == 1 && secondHand.GrabStrength == 1 && 200 < differ)
                    {
                        if (-70 < left_z && left_z < 80 && -70 < right_z && right_z < 80)
                        {
                            //Plain left right . 
                            if (left.PalmPosition.y > right.PalmPosition.y && lefty_righty > 80)
                            {
                                Debug.Log("Plain 오른쪽으로 이동 ");
                                appear(wheelControl2);
                                keybd_event(0x44, 0, 0x00, 0);
                                System.Threading.Thread.Sleep(100);
                                keybd_event(0x44, 0, 0x02, 0);
                                System.Threading.Thread.Sleep(50);

                            }
                            else if (left.PalmPosition.y < right.PalmPosition.y && righty_lefty > 80)
                            {
                                Debug.Log("Plain 왼쪽으로 이동 ");
                                appear(wheelControl);
                                keybd_event(0x41, 0, 0x00, 0);
                                System.Threading.Thread.Sleep(100);
                                keybd_event(0x41, 0, 0x02, 0);
                                System.Threading.Thread.Sleep(50);

                            }

                        }
                        else if (left_z < -70 && right_z < -70)
                        {
                            //forward left right . 
                            Debug.Log("forward 이동 ");
                            appear(wheelControl3);
                            keybd_event(0x57, 0, 0x00, 0);
                            System.Threading.Thread.Sleep(100);
                            keybd_event(0x57, 0, 0x02, 0);  // W 
                            System.Threading.Thread.Sleep(50);

                        }

                        else if (left_z > 90 && right_z > 90)
                        {
                            //backward left right . 
                            Debug.Log("backward 이동 ");
                            appear(wheelControl4);
                            keybd_event(0x53, 0, 0x00, 0);
                            System.Threading.Thread.Sleep(100);
                            keybd_event(0x53, 0, 0x02, 0);  // S 
                            System.Threading.Thread.Sleep(50);

                        }

                    }

                    // Debug.Log("right.palmposition.x" + right.PalmPosition.y);
                } //오른손이 먼저 들어올때
                else if (firstHand.IsLeft == false)
                {
                    right = firstHand;
                    left = secondHand;

                    left_z = left.PalmPosition.z;
                    right_z = right.PalmPosition.z;
                    left_y = left.PalmPosition.y;
                    right_y = right.PalmPosition.y;

                    differ = firstHand.PalmPosition.x - secondHand.PalmPosition.x;

                    if (firstHand.GrabStrength == 1 && secondHand.GrabStrength == 1 && 200 < differ)
                    {
                        Debug.Log("2");
                        if (-70 < left_z && left_z < 80 && -70 < right_z && right_z < 80)
                        {
                            //Plain left right . 
                            if (left.PalmPosition.y > right.PalmPosition.y && lefty_righty > 80)
                            {
                                Debug.Log("Plain 오른쪽으로 이동 ");
                                appear(wheelControl2);
                                keybd_event(0x44, 0, 0x00, 0);
                                System.Threading.Thread.Sleep(100);
                                keybd_event(0x44, 0, 0x02, 0);
                                System.Threading.Thread.Sleep(50);

                            }
                            else if (left.PalmPosition.y < right.PalmPosition.y && righty_lefty > 80)
                            {
                                Debug.Log("Plain 왼쪽으로 이동 ");
                                appear(wheelControl);
                                keybd_event(0x41, 0, 0x00, 0);
                                System.Threading.Thread.Sleep(100);
                                keybd_event(0x41, 0, 0x02, 0);
                                System.Threading.Thread.Sleep(50);

                            }

                        }
                        else if (left_z < -70 && right_z < -70)
                        {
                            //forward left right . 
                            Debug.Log("forward 이동 ");
                            appear(wheelControl3);
                            keybd_event(0x57, 0, 0x00, 0);
                            System.Threading.Thread.Sleep(100);
                            keybd_event(0x57, 0, 0x02, 0);  // W 
                            System.Threading.Thread.Sleep(50);

                        }
                        else if (left_z > 90 && right_z > 90)
                        {
                            //backward left right . 
                            Debug.Log("backward 이동 ");
                            appear(wheelControl4);
                            keybd_event(0x53, 0, 0x00, 0);
                            System.Threading.Thread.Sleep(100);
                            keybd_event(0x53, 0, 0x02, 0);  // S 
                            System.Threading.Thread.Sleep(50);

                        }

                    }
                }
            }
        }

        //only one hand. 
        if (frame.Hands.Count == 1)
        {
            float speed1 = firstHand.Fingers[1].TipVelocity.y;
            float speed2 = firstHand.Fingers[2].TipVelocity.y;
            Vector handSpeed = firstHand.PalmVelocity;
            
            //indexMode 속도 제어
            if (position.z < -170 && thumbDir.y > 0.15 && indexDir.y > 0.5 && thirdDir.y > 0.5 && fourthDir.y > 0.5 && babyDir.y > 0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
            {
                appear(speedUp);
                if (idx_ratio < 8.0f && idy_ratio < 8.0f)
                {
                    idx_ratio = idx_ratio + 0.5f;
                    idy_ratio = idy_ratio + 0.5f;
                    Debug.Log("up");
                }
            }
            else if (position.z < -170 && thumbDir.y < -0.15 && indexDir.y < -0.5 && thirdDir.y < -0.5 && fourthDir.y < -0.5 && babyDir.y < -0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
            {
                appear(speedDown);
                if (idx_ratio > 0.8f && idy_ratio > 0.8f)
                {
                    idx_ratio = idx_ratio - 0.5f;
                    idy_ratio = idy_ratio - 0.5f;
                    Debug.Log("down");
                }
            }
            //scroll 제어
            else if (thumbDir.y > 0.15 && indexDir.y > 0.5 && thirdDir.y > 0.5 && fourthDir.y > 0.5 && babyDir.y > 0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
            {
                appear(palmUp);
                //마우스 휠 up
                mouse_event(MOUSEEVENTF_WHEEL, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 60);
            }
            else if (thumbDir.y < -0.15 && indexDir.y < -0.5 && thirdDir.y < -0.5 && fourthDir.y < -0.5 && babyDir.y < -0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
            {
                appear(palmDown);
                //마우스 휠 down
                mouse_event(MOUSEEVENTF_WHEEL, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), -60);
            }
            
            //뒤로가기 앞으로가기
            if ((!firstHand.IsLeft && thumbDir.x > 0.1 && indexDir.x > 0.20 && thirdDir.x > 0.20 && fourthDir.x > 0.20 && babyDir.x > 0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || backPage1 == true)
            {
                //Debug.Log("1");
                backPage1 = true;
                if ((thumbDir.x < -0.1 && indexDir.x < -0.20 && thirdDir.x < -0.20 && fourthDir.x < -0.20 && babyDir.x < -0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || backPage2 == true)
                {
                    //Debug.Log("2");
                    backPage2 = true;
                    if (thumbDir.x < -0.35 && indexDir.x < -0.5 && thirdDir.x < -0.5 && fourthDir.x < -0.5 && babyDir.x < -0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                    {
                        //Debug.Log("3");
                        appear(backward);
                        keybd_event(0x12, 0, 0x00, 0);
                        keybd_event(0x25, 0, 0x00, 0);
                        keybd_event(0x25, 0, 0x02, 0);
                        keybd_event(0x12, 0, 0x02, 0);
                        backPage1 = false;
                        backPage2 = false;
                    }
                    else if (thumbDir.x > 0 && indexDir.x > 0 && thirdDir.x > 0 && fourthDir.x > 0 && babyDir.x > 0)
                    {
                        backPage1 = false;
                        backPage2 = false;
                        forwardPage1 = false;
                        forwardPage2 = false;
                    }
                }

            }

            else if ((firstHand.IsLeft && thumbDir.x < -0.1 && indexDir.x < -0.2 && thirdDir.x < -0.2 && fourthDir.x < -0.2 && babyDir.x < -0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || forwardPage1 == true)
            {
                forwardPage1 = true;
                //Debug.Log("4");
                if ((thumbDir.x > 0.1 && indexDir.x > 0.20 && thirdDir.x > 0.20 && fourthDir.x > 0.20 && babyDir.x > 0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || forwardPage2 == true)
                {
                    forwardPage2 = true;
                    //Debug.Log("5");
                    if (thumbDir.x > 0.35 && indexDir.x > 0.5 && thirdDir.x > 0.5 && fourthDir.x > 0.5 && babyDir.x > 0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                    {
                        //Debug.Log("6");
                        appear(forward);
                        keybd_event(0x12, 0, 0x00, 0);
                        keybd_event(0x27, 0, 0x00, 0);
                        keybd_event(0x27, 0, 0x02, 0);
                        keybd_event(0x12, 0, 0x02, 0);
                        forwardPage1 = false;
                        forwardPage2 = false;
                    }
                    else if (thumbDir.x < 0 && indexDir.x < 0 && thirdDir.x < 0 && fourthDir.x < 0 && babyDir.x < 0)
                    {
                        backPage1 = false;
                        backPage2 = false;
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

            
            //index move start

            if (!firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended)
            {
                if (indexPalmMove == false)
                {
                    indexMove = true;
                }
                else if (indexMove == false)
                {
                    indexPalmMove = true;
                }
            }
            else if (firstHand.Fingers[4].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended)
            {
                ILYgesture = true;
            }

            //calibration하고 나서 나머지 값들 초기화
            else
            {
                y_cali = currentResolutionCenterHeight - 200;
                ILYgesture = false;
            }
            
            //palm move calibration중
            if (ILYgesture == true && indexPalmMove == true)
            {
                appear(ILY);
                palm_x_now = position.x;
                if (palm_x_max < palm_x_now)
                {   // 최대< 현재
                    palm_x_max = palm_x_now;    //최대 <- 현재
                }
                else if (palm_x_min > palm_x_now)
                {
                    palm_x_min = palm_x_now;
                    //Debug.Log("Small Size ");
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
                
                // x, y range
                float x_leap = palm_x_max - palm_x_min;
                float y_leap = palm_y_max - palm_y_min;
                
                // x, y ratio
                x_ratio = currentResolutionWidth / x_leap;
                y_ratio = currentResolutionHeight / y_leap;

                cali_flag = true;
            }

            //Palm Mode Control.
            else if (indexPalmMove == true && !firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended)
            {
                if (cali_flag == true)
                {
                    y_cali = currentResolutionCenterHeight + 200;
                }
                appear(Pmove);
                //move
                SetCursorPos(currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y)); // this!
                if (dragAndDrop == true && firstHand.PalmPosition.z > -90)
                {
                    mouse_event(MOUSEEVENTF_LEFTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                    System.Threading.Thread.Sleep(100);
                    Debug.Log(" 드래그  떼어짐. . ");
                    dragAndDrop = false;
                }
                else if (firstHand.Fingers[1].IsExtended && firstHand.PalmPosition.z < -90 && dragAndDrop == false)
                {
                    appear(drag);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                    System.Threading.Thread.Sleep(100);
                    dragAndDrop = true;
                    Debug.Log(" 드래그  눌림. ");
                }
            }

            //IDXMODE

            //else if (indexMove && !firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended) //여기다 넣어요
            //{
            //    if (firstHand.Fingers[1].TipVelocity.Magnitude < 600)
            //    {
            //        appear(Imove);
            //        SetCursorPos(currentResolutionCenterWidth + (int)xSetting(firstHand), currentResolutionCenterHeight + (int)ySetting(firstHand));
            //    }
            //    if (dragAndDrop == true && firstHand.PalmPosition.z > -80)
            //    {
            //        mouse_event(MOUSEEVENTF_LEFTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
            //        System.Threading.Thread.Sleep(100);
            //        Debug.Log(" 드래그  떼어짐. . ");
            //        dragAndDrop = false;
            //    }
            //    else if (firstHand.Fingers[1].IsExtended && firstHand.PalmPosition.z < -80 && dragAndDrop == false)
            //    {
            //        appear(drag);
            //        mouse_event(MOUSEEVENTF_LEFTDOWN, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
            //        System.Threading.Thread.Sleep(100);
            //        dragAndDrop = true;
            //        Debug.Log(" 드래그  눌림. ");
            //    }

            //}

            else if (indexMove && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended) //여기다 넣어요
            {
                if (firstHand.Fingers[1].TipVelocity.Magnitude < 600)
                {
                    appear(Imove);
                    SetCursorPos(currentResolutionCenterWidth + (int)xSetting(firstHand), currentResolutionCenterHeight + (int)ySetting(firstHand));
                }
                if (dragAndDrop == true && !firstHand.Fingers[0].IsExtended)
                {
                    mouse_event(MOUSEEVENTF_LEFTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                    System.Threading.Thread.Sleep(100);
                    Debug.Log(" 드래그  떼어짐. . ");
                    dragAndDrop = false;
                }
                else if (firstHand.Fingers[1].IsExtended && firstHand.Fingers[0].IsExtended && dragAndDrop == false)
                {
                    appear(drag);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                    System.Threading.Thread.Sleep(100);
                    dragAndDrop = true;
                    Debug.Log(" 드래그  눌림. ");
                }

            }

            //우클릭 .
            if ((!firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended) && speed2 < -500 && speed1 < -500 && handSpeed[0] < 150 && handSpeed[1] < 150 && handSpeed[2] < 150 && handSpeed[0] > -150 && handSpeed[1] > -150 && handSpeed[2] > -150)
            {
                appear(twoFinger);
                Debug.Log("우클릭");
                mouse_event(MOUSEEVENTF_RIGHTDOWN, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                System.Threading.Thread.Sleep(100);
                mouse_event(MOUSEEVENTF_RIGHTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                System.Threading.Thread.Sleep(150);
            }

            //좌클릭
            else if ((!firstHand.Fingers[0].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended) && speed1 < -800 && speed2 >-300 && handSpeed[0] < 150 && handSpeed[1] < 150 && handSpeed[2] < 150 && handSpeed[0] > -150 && handSpeed[1] > -150 && handSpeed[2] > -150 && indexDir.y < -0.75)
            {
                Debug.Log("좌클릭");
                appear(Iclick);
                mouse_event(MOUSEEVENTF_LEFTDOWN, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                System.Threading.Thread.Sleep(100);
                mouse_event(MOUSEEVENTF_LEFTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                System.Threading.Thread.Sleep(100);
            }

        }


    }


    public void speedReset()
    {
        palm_x_min = 0.0f;
        palm_y_max = 0.0f;
        palm_y_min = currentResolutionCenterHeight;

        x_ratio = 4.0f;
        y_ratio = 3.0f;
        cali_flag = false;

        idx_speed = 0.0f;
        idy_speed = 0.0f;
        
        idx_ratio = 2.0f;
        idy_ratio = 2.0f;
    }

    public float xSetting(Hand hand)
    {

        if (hand.Fingers[1].Direction.x < -0.2) //&& idx_speed >= -currentResolutionCenterWidth)
        {
            //감소
            idx_speed = idx_speed - (1 * idx_ratio);
        }
        else if (hand.Fingers[1].Direction.x > 0.2) //&& idx_speed <= currentResolutionCenterWidth)
        {
            //증가
            idx_speed = idx_speed + (1 * idx_ratio);
        }
        return idx_speed;
    }

    public float ySetting(Hand hand)
    {
        if (hand.Fingers[1].Direction.y < -0.25 && idy_speed <= currentResolutionCenterHeight)
        {
            idy_speed = idy_speed + (1 * idy_ratio);
        }
        else if (hand.Fingers[1].Direction.y > 0.35 && idy_speed >= -currentResolutionCenterHeight)
        {
            idy_speed = idy_speed - (1 * idy_ratio);
        }
        return idy_speed;
    }


    public void appear(GameObject obj)
    {
        rewind.transform.position = new Vector3(0f, 0f, 2.0f);
        modechange.transform.position = new Vector3(0f, 0f, 2.0f);
        twoFinger.transform.position = new Vector3(0f, 0f, 2.0f);
        threeFinger.transform.position = new Vector3(0f, 0f, 2.0f);
        Iclick.transform.position = new Vector3(0f, 0f, 2.0f);
        Pclick.transform.position = new Vector3(0f, 0f, 2.0f);
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
        drag.transform.position = new Vector3(0f, 0f, 2.0f);
        

        obj.transform.position = new Vector3(0f, 0f, 0.005f);
        //mouseMove.transform.position = new Vector3(0f, 0f, 2.0f);

    }

}


