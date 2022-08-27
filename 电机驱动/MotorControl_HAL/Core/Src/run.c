#include "run.h"

uint8_t RecBuff;            //数据接收
uint8_t DataBuff[64] = {0}; //数据接收缓冲
uint8_t RecCounter = 0;     //串口接收技术

float Motor_TargetSpeed[4];
float Motor_SetSpeed[4];
uint8_t Motor_FromMain[5]; // 0~3 电机1234   5 舵机

uint16_t over_flag = 0;

void Motor_Init(void)
{
    //开启定时器的PWM输出
    HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_1);
    HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_2);
    HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_3);
    HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_4);

    HAL_TIM_PWM_Start(&htim4, TIM_CHANNEL_1);
    HAL_TIM_PWM_Start(&htim4, TIM_CHANNEL_2);
    HAL_TIM_PWM_Start(&htim4, TIM_CHANNEL_3);
    HAL_TIM_PWM_Start(&htim4, TIM_CHANNEL_4);

    HAL_TIM_PWM_Start(&htim2, TIM_CHANNEL_3);

    //初始化电机
    __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_1, 0);
    __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_2, 0);
    __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_3, 0);
    __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_4, 0);

    __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_1, 0);
    __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_2, 0);
    __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_3, 0);
    __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_4, 0);

    __HAL_TIM_SetCompare(&htim2, TIM_CHANNEL_3, ServoOpen);
}

void Motor_Regulate(void)
{
    static float Motor_SetSpeed_D[4];
    static float Motor_SetSpeed_L[4];

    int i = 0;
    for (i = 0; i < 4; i++)
    {
        //渐进变速，防止电机烧毁
        if (Motor_SetSpeed[i] < Motor_TargetSpeed[i] - acculation || Motor_SetSpeed[i] > Motor_TargetSpeed[i] + acculation)
        {
            Motor_SetSpeed[i] += acculation_P * (Motor_TargetSpeed[i] - Motor_SetSpeed[i]) + acculation_D * Motor_SetSpeed_D[i];
        }
        else
        {
            Motor_SetSpeed[i] = Motor_TargetSpeed[i];
        }

        Motor_SetSpeed_D[i] = Motor_SetSpeed[i] - Motor_SetSpeed_L[i];
        Motor_SetSpeed_L[i] = Motor_SetSpeed[i];

        //设置PWM占空比上下限，防止电调暴毙
        if (Motor_SetSpeed[i] > MaxMotorSpeed)
        {
            Motor_SetSpeed[i] = MaxMotorSpeed;
        }
    }

    //将电机速度转化为PWM输出
    //电机1 =============================================================
    if (Motor_SetSpeed[0] > MaxMotorSpeed / 2)
    {
        __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_1, (int)(2 * Motor_SetSpeed[0] - MaxMotorSpeed));
        __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_2, 0);
    }
    else if (Motor_SetSpeed[0] < MaxMotorSpeed / 2)
    {
        __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_2, (int)(MaxMotorSpeed - 2 * Motor_SetSpeed[0]));
        __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_1, 0);
    }
    else
    {
        __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_1, 0);
        __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_2, 0);
    }
    //电机2 =============================================================
    if (Motor_SetSpeed[1] > MaxMotorSpeed / 2)
    {
        __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_3, (int)(2 * Motor_SetSpeed[1] - MaxMotorSpeed));
        __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_4, 0);
    }
    else if (Motor_SetSpeed[1] < MaxMotorSpeed / 2)
    {
        __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_4, (int)(MaxMotorSpeed - 2 * Motor_SetSpeed[1]));
        __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_3, 0);
    }
    else
    {
        __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_3, 0);
        __HAL_TIM_SetCompare(&htim4, TIM_CHANNEL_4, 0);
    }
    //电机3 =============================================================
    if (Motor_SetSpeed[2] > MaxMotorSpeed / 2)
    {
        __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_1, (int)(2 * Motor_SetSpeed[2] - MaxMotorSpeed));
        __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_2, 0);
    }
    else if (Motor_SetSpeed[2] < MaxMotorSpeed / 2)
    {
        __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_2, (int)(MaxMotorSpeed - 2 * Motor_SetSpeed[2]));
        __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_1, 0);
    }
    else
    {
        __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_1, 0);
        __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_2, 0);
    }
    //电机4 =============================================================
    if (Motor_SetSpeed[3] > MaxMotorSpeed / 2)
    {
        __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_3, (int)(2 * Motor_SetSpeed[3] - MaxMotorSpeed));
        __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_4, 0);
    }
    else if (Motor_SetSpeed[3] < MaxMotorSpeed / 2)
    {
        __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_4, (int)(MaxMotorSpeed - 2 * Motor_SetSpeed[3]));
        __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_3, 0);
    }
    else
    {
        __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_3, 0);
        __HAL_TIM_SetCompare(&htim3, TIM_CHANNEL_4, 0);
    }
    //    =============================================================
}

void UART_Translate(void) //对串口传递的电机信息进行转换，作校验，并输出
{
    Motor_TargetSpeed[0] = MaxMotorSpeed - (Motor_FromMain[0] * MaxMotorSpeed / 200); //将0~200之间的值转化成0~MaxMotorSpeed
    Motor_TargetSpeed[3] = (Motor_FromMain[1] * MaxMotorSpeed / 200); //将0~200之间的值转化成0~MaxMotorSpeed
    Motor_TargetSpeed[2] = MaxMotorSpeed - (Motor_FromMain[2] * MaxMotorSpeed / 200); //将0~200之间的值转化成0~MaxMotorSpeed
    Motor_TargetSpeed[1] = MaxMotorSpeed - (Motor_FromMain[3] * MaxMotorSpeed / 200); //将0~200之间的值转化成0~MaxMotorSpeed
}

void Servo_Control(uint8_t servo_state)
{
    if (servo_state == 1)
        __HAL_TIM_SetCompare(&htim2, TIM_CHANNEL_3, ServoOpen);
    if (servo_state == 0)
        __HAL_TIM_SetCompare(&htim2, TIM_CHANNEL_3, ServoClose);
}


