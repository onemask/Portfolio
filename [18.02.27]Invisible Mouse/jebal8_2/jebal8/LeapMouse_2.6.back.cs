
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

    float HandPalmPitch;
    float HandPalmYaw;
    float HandPalmRoll;
    float HandWristRot;

    bool indexPalmMove;
    bool closedFist;
    bool extendedThumb;
    bool ILYgesture;
    bool cali_flag;


    bool indexMove;
    bool indexCali;
    bool indexCaliPlus;
    bool indexCaliMinus;
    bool indexCaliCondition1;
    bool indexCaliCondition2;

    // for calibration
    int rangeX;
    int rangeY;
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

    Vector2 mousePosition;

    int prePosX;
    int prePosY;

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
    bool fingerRotateR1;
    bool fingerRotateR2;

    bool fingerRotateL1;
    bool fingerRotateL2;

    bool backPage1;
    bool backPage2;
    bool fastForwardPage1;
    bool fastForwardPage2;

    bool leftClap;
    bool rightClap;

    bool firstIndex;

    void Start()
    {
        controller = new Controller();

        //Screen size setting
        Screen.SetResolution(120 * 16 / 9, 120, false);

        //initialization x,y
        currentResolutionWidth = Screen.currentResolution.width;  //해상도 x
        currentResolutionHeight = Screen.currentResolution.height;  // 해상도y
        currentResolutionCenterWidth = currentResolutionWidth / 2;
        currentResolutionCenterHeight = currentResolutionHeight / 2;
        prePosX = currentResolutionCenterWidth;
        prePosY = currentResolutionCenterHeight;

        //association cali
        palm_x_min = 0.0f;
        palm_y_max = 0.0f;
        palm_y_min = currentResolutionCenterHeight;

        //mouse moving value
        x_ratio = 4.0f;
        y_ratio = 3.0f;
        cali_flag = false;

        //rotate
        fingerRotateR1 = false;
        fingerRotateR2 = false;
        
        fingerRotateL1 = false;
        fingerRotateL2 = false;

        backPage1 = false;
        backPage2 = false;
        fastForwardPage1 = false;
        fastForwardPage2 = false;

        firstIndex = false;
        

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
        forward = GameObject.Find("fastForward") as GameObject;

        idx_speed = 0.0f;
        idy_speed = 0.0f;

        indexCaliPlus = false;
        indexCaliMinus = false;
        idx_ratio = 1.0f;
        idy_ratio = 1.0f;

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

        Vector position = firstHand.PalmPosition;

        Vector thumbDir = firstHand.Fingers[0].Direction;
        Vector indexDir = firstHand.Fingers[1].Direction;
        Vector thirdDir = firstHand.Fingers[2].Direction;
        Vector fourthDir = firstHand.Fingers[3].Direction;
        Vector babyDir = firstHand.Fingers[4].Direction;
        //Vector thumbPos = firstHand.Fingers[0].TipPosition;
        //Vector indexPos = firstHand.Fingers[1].TipPosition;
        //Vector thirdPos = firstHand.Fingers[2].TipPosition;
        //Vector fourthPos = firstHand.Fingers[3].TipPosition;
        //Vector babyPos = firstHand.Fingers[4].TipPosition;

        //Debug.Log("w"+firstHand.Fingers[1].bones[3].Rotation.w);
        //Debug.Log("x" + firstHand.Fingers[1].bones[3].Rotation.x);
        //Debug.Log("y" + firstHand.Fingers[1].bones[3].Rotation.y);
        //Debug.Log("z" + firstHand.Fingers[1].bones[3].Rotation.z);
        //Debug.Log("w" + firstHand.Rotation.w);
        //Debug.Log("now" + controller.Now());

        Debug.Log(firstHand.TimeVisible);

        //application quit
        if (secondHand != null)
        {
            if (firstHand.IsLeft == true)
            {
                if (firstHand.PalmVelocity.x > 500 && secondHand.PalmVelocity.x < -500 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
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
                        Application.Quit();
                    }
                }
            }
            else
            {
                if (firstHand.PalmVelocity.x < -500 && secondHand.PalmVelocity.x > 500 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
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
                        Application.Quit();
                    }
                }
            }

            //mode change 
            //모드 체인지에서 인덱스 모션 아닐때 초기화 추가 첫번째 조건 
            if ((!firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended) || firstIndex == true)
            {
                firstIndex = true;
                //Debug.Log("1");
                if ((!secondHand.Fingers[0].IsExtended && !secondHand.Fingers[1].IsExtended && !secondHand.Fingers[2].IsExtended && !secondHand.Fingers[3].IsExtended && !secondHand.Fingers[4].IsExtended && !firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended) && (indexMove == true || indexPalmMove == true))
                {
                    //Debug.Log("2");
                    if (!firstHand.Fingers[0].IsExtended && !firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended)
                    {
                        speedReset();
                        appear(modechange);
                        if (indexMove == true)
                        {
                            //Debug.Log("5");
                            indexMove = false;
                            indexPalmMove = true;
                        }
                        else if (indexPalmMove == true)
                        {
                            //Debug.Log("6");
                            indexPalmMove = false;
                            indexMove = true;
                        }
                        firstIndex = false;
                        System.Threading.Thread.Sleep(200);

                    }
                }

            }

            if (!secondHand.Fingers[0].IsExtended && !secondHand.Fingers[1].IsExtended && !secondHand.Fingers[2].IsExtended && !secondHand.Fingers[3].IsExtended && !secondHand.Fingers[4].IsExtended)
            {
                if (thumbDir.y > 0.15 && indexDir.y > 0.45 && thirdDir.y > 0.45 && fourthDir.y > 0.45 && babyDir.y > 0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                {
                    appear(speedUp);
                    indexCaliPlus = true;

                }
                else if (thumbDir.y < -0.15 && indexDir.y < -0.45 && thirdDir.y < -0.45 && fourthDir.y < -0.45 && babyDir.y < -0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                {
                    appear(speedDown);
                    indexCaliMinus = true;
                }

                //rewind fastForward
                if ((!firstHand.IsLeft && indexDir.x > 0.20 && thirdDir.x > 0.20 && fourthDir.x > 0.20 && babyDir.x > 0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || fingerRotateL1 == true)
                {
                    //Debug.Log("1");
                    fingerRotateL1 = true;
                    if ((indexDir.x < -0.20 && thirdDir.x < -0.20 && fourthDir.x < -0.20 && babyDir.x < -0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || fingerRotateL2 == true)
                    {
                        //Debug.Log("2");
                        fingerRotateL2 = true;
                        if (indexDir.x < -0.5 && thirdDir.x < -0.5 && fourthDir.x < -0.5 && babyDir.x < -0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                        {
                            //Debug.Log("3");
                            appear(rewind);
                            keybd_event(0x4A, 0, 0x00, 0);
                            keybd_event(0x4A, 0, 0x02, 0);
                            fingerRotateL1 = false;
                            fingerRotateL2 = false;
                        }
                        else if (indexDir.x > 0 && thirdDir.x > 0 && fourthDir.x > 0 && babyDir.x > 0)
                        {
                            fingerRotateL1 = false;
                            fingerRotateL2 = false;
                            fingerRotateR1 = false;
                            fingerRotateR2 = false;
                        }
                    }

                }

                else if ((firstHand.IsLeft && indexDir.x < -0.2 && thirdDir.x < -0.2 && fourthDir.x < -0.2 && babyDir.x < -0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || fingerRotateR1 == true)
                {
                    fingerRotateR1 = true;
                    //Debug.Log("4");
                    if ((indexDir.x > 0.20 && thirdDir.x > 0.20 && fourthDir.x > 0.20 && babyDir.x > 0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || fingerRotateR2 == true)
                    {
                        fingerRotateR2 = true;
                        //Debug.Log("5");
                        if (indexDir.x > 0.5 && thirdDir.x > 0.5 && fourthDir.x > 0.5 && babyDir.x > 0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                        {
                            //Debug.Log("6");
                            appear(fastForward);
                            keybd_event(0x4C, 0, 0x00, 0);
                            keybd_event(0x4C, 0, 0x02, 0);
                            fingerRotateR1 = false;
                            fingerRotateR2 = false;
                        }
                        else if (indexDir.x < 0 && thirdDir.x < 0 && fourthDir.x < 0 && babyDir.x < 0)
                        {
                            fingerRotateL1 = false;
                            fingerRotateL2 = false;
                            fingerRotateR1 = false;
                            fingerRotateR2 = false;
                        }
                    }


                }
                else
                {
                    fingerRotateL1 = false;
                    fingerRotateL2 = false;
                    fingerRotateR1 = false;
                    fingerRotateR2 = false;
                }
            }
            //index move calibration 속도 조절
            if (indexCaliPlus == true && idx_ratio < 8.0f && idy_ratio < 8.0f)
            {
                appear(speedUp);
                idx_ratio = idx_ratio + 0.5f;
                idy_ratio = idy_ratio + 0.5f;
                indexCaliPlus = false;
                Debug.Log("up");
            }
            else if (indexCaliMinus == true && idx_ratio > 0.8f && idy_ratio > 0.8f)
            {
                appear(speedDown);
                idx_ratio = idx_ratio - 0.5f;
                idy_ratio = idy_ratio - 0.5f;
                indexCaliMinus = false;
                Debug.Log("down");
            }



        }

        if (secondHand == null)
        {
            firstIndex = false;
            //scroll 제어
            if (thumbDir.y > 0.15 && indexDir.y > 0.5 && thirdDir.y > 0.5 && fourthDir.y > 0.5 && babyDir.y > 0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
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
                        fastForwardPage1 = false;
                        fastForwardPage2 = false;
                    }
                }

            }

            else if ((firstHand.IsLeft && thumbDir.x < -0.1 && indexDir.x < -0.2 && thirdDir.x < -0.2 && fourthDir.x < -0.2 && babyDir.x < -0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || fastForwardPage1 == true)
            {
                fastForwardPage1 = true;
                //Debug.Log("4");
                if ((thumbDir.x > 0.1 && indexDir.x > 0.20 && thirdDir.x > 0.20 && fourthDir.x > 0.20 && babyDir.x > 0.1 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended) || fastForwardPage2 == true)
                {
                    fastForwardPage2 = true;
                    //Debug.Log("5");
                    if (thumbDir.x > 0.35 && indexDir.x > 0.5 && thirdDir.x > 0.5 && fourthDir.x > 0.5 && babyDir.x > 0.35 && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
                    {
                        //Debug.Log("6");
                        appear(forward);
                        keybd_event(0x12, 0, 0x00, 0);
                        keybd_event(0x27, 0, 0x00, 0);
                        keybd_event(0x27, 0, 0x02, 0);
                        keybd_event(0x12, 0, 0x02, 0);
                        fastForwardPage1 = false;
                        fastForwardPage2 = false;
                    }
                    else if (thumbDir.x < 0 && indexDir.x < 0 && thirdDir.x < 0 && fourthDir.x < 0 && babyDir.x < 0)
                    {
                        backPage1 = false;
                        backPage2 = false;
                        fastForwardPage1 = false;
                        fastForwardPage2 = false;
                    }
                }


            }
            else
            {
                backPage1 = false;
                backPage2 = false;
                fastForwardPage1 = false;
                fastForwardPage2 = false;
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
                closedFist = false;
                extendedThumb = false;
                ILYgesture = false;
                indexCali = false;
            }

            
            //palm move calibration중
            if (ILYgesture == true && indexPalmMove == true)
            {
                appear(ILY);
                // x range calculation , x_ max=o.of ,x_min=해상도
                palm_x_now = position.x;
                if (palm_x_max < palm_x_now)
                {   // 최대< 현재
                    palm_x_max = palm_x_now;    //최대 <- 현재
                                                //Debug.Log("Big Size ");
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

                //Debug.Log("palm_x_max: " + palm_x_max);
                //Debug.Log("palm_x_min: " + palm_x_min);
                //Debug.Log("palm_y_max: " + palm_y_max);
                //Debug.Log("palm_y_min: " + palm_y_min);
                // x, y range
                float x_leap = palm_x_max - palm_x_min;
                float y_leap = palm_y_max - palm_y_min;

                //Debug.Log("x_leap: " + x_leap);
                //Debug.Log("y_leap: " + y_leap);

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
                Debug.Log("손바닥 속도" + hands[0].PalmVelocity);

                Vector thumb = hands[0].Fingers[0].TipVelocity;
                float thumbSpeed = thumb.Magnitude;
                Debug.Log("엄지 속도" + thumbSpeed);


                Vector velocity0 = hands[0].Fingers[0].TipVelocity;
                float speed0 = velocity0.Magnitude;
                Vector velocity1 = hands[0].Fingers[1].TipVelocity;
                float speed1 = velocity1.Magnitude;
                Vector velocity2 = hands[0].Fingers[2].TipVelocity;
                float speed2 = velocity2.Magnitude;


                //Debug.Log("tip speed " + speed);

                Vector handSpeed = hands[0].PalmVelocity;
                //Debug.Log("hands speed " + handSpeed);

                //우클릭 
                if ((!firstHand.Fingers[0].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended) && speed2 > 800 && speed1 > 800 && handSpeed[0] < 300 && handSpeed[1] < 300 && handSpeed[2] < 200 && handSpeed[0] > -300 && handSpeed[1] > -300 && handSpeed[2] > -200)
                {
                    appear(twoFinger);
                    Debug.Log("우클릭");
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                    System.Threading.Thread.Sleep(100);
                    mouse_event(MOUSEEVENTF_RIGHTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                    System.Threading.Thread.Sleep(150);
                }

                //좌클릭 
                else if ((!firstHand.Fingers[0].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended) && speed1 > 1000 && handSpeed[0] < 300 && handSpeed[1] < 300 && handSpeed[2] < 200 && handSpeed[0] > -300 && handSpeed[1] > -300 && handSpeed[2] > -200)
                {
                    Debug.Log("좌클릭");
                    appear(Iclick);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                    System.Threading.Thread.Sleep(100);
                    mouse_event(MOUSEEVENTF_LEFTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                    System.Threading.Thread.Sleep(150);
                }


            }

            //IDXMODE
            
            else if (indexMove && !firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended) //여기다 넣어요
            {
                Vector direction = hands[0].Fingers[1].Direction;
                //Debug.Log("direction " + direction);

                Vector tipPosition0 = hands[0].Fingers[0].TipPosition;
                Vector tipPosition1 = hands[0].Fingers[1].TipPosition;
                Vector tipPosition2 = hands[0].Fingers[2].TipPosition;

                Vector thumb = hands[0].Fingers[0].TipVelocity;
                float thumbSpeed = thumb.Magnitude;

                Vector velocity0 = hands[0].Fingers[0].TipVelocity;
                float speed0 = velocity0.Magnitude;
                Vector velocity1 = hands[0].Fingers[1].TipVelocity;
                float speed1 = velocity1.Magnitude;
                Vector velocity2 = hands[0].Fingers[2].TipVelocity;
                float speed2 = velocity2.Magnitude;
                Vector handSpeed = hands[0].PalmVelocity;

                if (firstHand.Fingers[1].TipVelocity.Magnitude < 600)
                {
                    appear(Imove);
                    SetCursorPos(currentResolutionCenterWidth + (int)xSetting(firstHand), currentResolutionCenterHeight + (int)ySetting(firstHand));

                }


                //우클릭 .  
                if ((!firstHand.Fingers[0].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended) && speed2 > 800 && speed1 > 800 && handSpeed[0] < 300 && handSpeed[1] < 300 && handSpeed[2] < 200 && handSpeed[0] > -300 && handSpeed[1] > -300 && handSpeed[2] > -200)
                {
                    appear(twoFinger);
                    Debug.Log("우클릭");
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                    System.Threading.Thread.Sleep(100);
                    mouse_event(MOUSEEVENTF_RIGHTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                    System.Threading.Thread.Sleep(150);
                }

                //좌클릭 
                else if ((!firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended) && speed1 > 1000 && handSpeed[0] < 300 && handSpeed[1] < 300 && handSpeed[2] < 200 && handSpeed[0] > -300 && handSpeed[1] > -300 && handSpeed[2] > -200)
                {
                    Debug.Log("좌클릭");
                    appear(Iclick);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                    System.Threading.Thread.Sleep(100);
                    mouse_event(MOUSEEVENTF_LEFTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                    System.Threading.Thread.Sleep(150);
                }





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

        indexCaliPlus = false;
        indexCaliMinus = false;
        idx_ratio = 1.0f;
        idy_ratio = 1.0f;
    }

    public float xSetting(Hand hand)
    {

        if (hand.Fingers[1].Direction.x < -0.25) //&& idx_speed >= -currentResolutionCenterWidth)
        {
            //감소하는 손가락 
            idx_speed = idx_speed - (1 * idx_ratio);
        }
        else if (hand.Fingers[1].Direction.x > 0.25) //&& idx_speed <= currentResolutionCenterWidth)
        {
            //증가하는 손가락 
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
        else if (hand.Fingers[1].Direction.y > 0.25 && idy_speed >= -currentResolutionCenterHeight)
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

        obj.transform.position = new Vector3(0f, 0f, 0.005f);
        //mouseMove.transform.position = new Vector3(0f, 0f, 2.0f);

    }

}


