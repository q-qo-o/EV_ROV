#ifndef __RUN_H
#define __RUN_H

#include "tim.h"

#include <stdio.h>
#include <stdlib.h>
//#define Regulate_Speed 0x0a//PWM调速的速度
#define MaxMotorSpeed 4000//PWM上限
#define acculation_P 0.5
#define acculation_D -0.01
#define acculation 80


#define ServoOpen 1200
#define ServoClose 2200


extern uint8_t RecBuff;//全局变量，串口接收
extern uint8_t DataBuff[64];
extern uint8_t RecCounter;//串口接收计数
extern uint16_t over_flag;//串口接收超时计数

extern float Motor_TargetSpeed[4];//电机目标速度
extern float Motor_SetSpeed[4];//电机设置的速度
extern uint8_t Motor_FromMain[5];;//主控设置的电机速度

void Motor_Init(void);//电机初始化
void Motor_Regulate(void);//电机调速
void UART_Translate(void); //对串口传递的电机信息进行转换，作校验，并输出
void Servo_Control(uint8_t servo_state);//舵机控制

#endif
