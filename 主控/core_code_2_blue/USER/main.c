#include "sys.h"
#include "delay.h"
#include "usart.h"
#include "flash.h"
#include "usart6.h"
#include "string.h"
#include "ms5805.h"
#include "myiic.h"
#include "stm32f4xx.h"
#include "pid.h"
#include "usart2_MEMS.h"
#include "tim.h"

u8 usart6_send[10] = {0xaf, 0x64, 0x64, 0x64, 0x64, 0x00, 0x00, 0x00, 0x00, 0xaf};
extern float data_to_flash[12];
float flash_read[12] = {0};
extern u8 usart1_rebuf[40];
extern u8 PC_to_MCU[40];
extern float main_to_motorf[10];
extern u8 main_to_motoru8[10];
extern float real_deep;
extern MEMS_struct mems;
extern u8 code_flag;
extern u8 time_count;
mystruct deep, yaw;
float test3;
int main(void)
{

	// int i=0;
	// int n=1;                                                     //通过调n的值来取不同区段的数据
	delay_init(168); //延时初始化
	IIC_Init();
	MS5837_PROM_Read();
	NVIC_PriorityGroupConfig(NVIC_PriorityGroup_2); //设置系统中断优先级分组2
	usart6_init();
	uart_DMA_init(115200);	  //串口初始化波特率为115200
	usart2_init();			  //姿态串口的初始化
	TIM3_Int_Init(49999, 83); // 50ms
	refresh_from_flash();	  //进入主函数先更新PID的值
							  //				   flash_data_conduct();
	while (1)
	{
		deep_out();			 //读取深度数据
		MEMS_DATA_conduct(); //姿态数据的处理
		motor_data_conduct();
		// 50ms执行一次
		if (code_flag == 1)
		{
			switch (PC_to_MCU[2])
			{
				//开环模式
			case 0x00:
				float_to_u8();
				memcpy(usart6_send, main_to_motoru8, 10);
				break;
				//深度模式
			case 0x02:
				deep.real = real_deep;

				if (PC_to_MCU[6] == 0x64) //如果上位机要求有垂直运动，保证目标值等于实际值，深度不起作用
				{
					main_to_motorf[3] = 100 + deep_PD(deep.target, deep.real);
					main_to_motorf[4] = 100 + deep_PD(deep.target, deep.real);
				}
				else
				{
					deep.target = real_deep;
				}
				float_to_u8();
				memcpy(usart6_send, main_to_motoru8, 10);
				break;
				//姿态加深度模式
			case 0x01:
				//深度
				if (PC_to_MCU[6] == 0x64) //如果上位机要求有垂直运动，保证目标值等于实际值，深度不起作用
					deep.real = real_deep;
				else
				{
					deep.real = real_deep;
					deep.target = real_deep; //
				}
				main_to_motorf[3] -= deep_PD(deep.target, deep.real);
				main_to_motorf[4] -= deep_PD(deep.target, deep.real);
				main_to_motorf[3] = 200 - main_to_motorf[3];

				float_to_u8();
				memcpy(usart6_send, main_to_motoru8, 10);
				//航向
				if (PC_to_MCU[4] == 0x64)
					yaw.real = mems.yaw; //这句话保证在移动的时候可以改变目标值
				else
				{
					yaw.real = mems.yaw;
					yaw.target = mems.yaw; //
				}
				main_to_motorf[1] += yaw_PD(yaw.target, yaw.real);
				main_to_motorf[2] -= yaw_PD(yaw.target, yaw.real);
				test3 = yaw_PD(yaw.target, yaw.real);
				;
				float_to_u8();
				memcpy(usart6_send, main_to_motoru8, 10);
				break;
			case 0x03:
				flash_data_conduct();
				break;
			default:
				__nop();
			}
			usart6_send_data(usart6_send, 10);
			code_flag = 0;
		}
		// 1s上发一次
		if (time_count > 10)
		{
			time_count = 0;
			send_to_PC();
		}
	}
}
