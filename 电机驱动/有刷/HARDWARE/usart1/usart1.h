#ifndef __usart_h
#define __usart_h
#include "sys.h"

#include <stdio.h>
#include <stdlib.h>

#define moter1 TIM4->CCR1
#define moter2 TIM4->CCR2
#define moter3 TIM4->CCR3
#define moter4 TIM4->CCR4
#define moter5 TIM3->CCR1
#define moter6 TIM3->CCR2
#define moter7 TIM3->CCR3
#define moter8 TIM3->CCR4
#define servo TIM2->CCR3

#define max 6000
#define speed 0.1
#define open 1200
#define close 2200
// 1.5 2.6 3.8 4.7

void translate(void);

void MOTOR_Init(void);
void usart1_init(void); //串口的初始化
#endif
