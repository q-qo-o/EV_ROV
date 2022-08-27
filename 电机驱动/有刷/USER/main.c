#include "delay.h"
#include "key.h"
#include "led.h"
#include "sys.h"
#include "usart1.h"
#include "timer.h"

u8 a, b;
u8 CANBUF_1[8];
u8 data[8];
u8 data1[8];
u16 pwm = 1500;

int main(void)
{
	delay_init(); //延时函数初始化
	LED_Init();
	TIM3_4_PWM_Init(19999, 71); //周期为20ms		要改

	MOTOR_Init();
	NVIC_PriorityGroupConfig(NVIC_PriorityGroup_2); //设置中断优先级分组为组2：2位抢占优先级，2位响应优先级
	usart1_init();
	LED0 = 1;
	while (1)
	{
		translate();

		delay_ms(10);
	}
}
