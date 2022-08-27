#ifndef __usart_h
#define __usart_h
#include "sys.h"
#define moter3  TIM4->CCR4
#define moter4  TIM4->CCR3
#define moter1  TIM4->CCR1
#define moter2  TIM4->CCR2

void translate(void);

void MOTOR_Init(void);
void usart1_init(void);    //串口的初始化
#endif


