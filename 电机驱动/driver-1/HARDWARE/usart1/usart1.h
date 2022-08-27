#ifndef __usart_h
#define __usart_h
#include "sys.h"

#define moter3 TIM4->CCR4
#define moter4 TIM4->CCR3
#define moter1 TIM4->CCR1
#define moter2 TIM4->CCR2
#define servo_pwm TIM3->CCR3

#define servo_open 1200
#define servo_close 2200

#include <stdio.h>
#include <stdlib.h>

void translate(void);

void MOTOR_Init(void);
void usart1_init(void);
#endif
