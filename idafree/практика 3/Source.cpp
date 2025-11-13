#include <stdio.h>

// Visual Studio Developer Command Prompt
// cl.exe /Zi source.cpp

int main() {

	int first, second, result;
	char operation;

	scanf("%d%c%d", &first, &operation, &second);

	__asm {
		mov eax, first;
		mov ebx, second;
		movzx ecx, operation;
		cmp ecx, '+';
		jnz subs;
		add eax, ebx;
		jmp res;
	subs:
		sub eax, ebx;
	res:
		mov result, eax;
	};

	printf("result = %d!", result);

	return 0;
};