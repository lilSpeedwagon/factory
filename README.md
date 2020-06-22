# Factory

Factory is a real-time managment strategy about automatization of modern fabrique. Build your custom conveyer line, process raw materials to make quality goods and sell it. But your resourses is limited. Manage energy consumption and keep production lean. Key feature of the game is automation. It is provided by custom programming language **BeltScript** for programming of in-game Programming Logic Controllers (PLC) and dynamic configuration of your production line.

### BeltScript

Custom programming language for writing simple code for execution on in-game PLC-like objects. Main purpose of the code is real-time calculating of output signals on the base of input signals of the PLC. The language is compiled (need performance for real-time systems) and dynamic-typed (for simplicity). Written in C++, [Catch2](https://github.com/catchorg/Catch2) for unit tests.

Example of code:
```
// print all odd numbers from 0 to 99
i = 0;
while (i < 100)
{
	if (i % 2 == 0)
	{
		print(i);
	}
	i = i + 1;
}
```

### Purposes
The game is developed as a Master Degree project in educational purposes.

### Written in:
- Game - Unity / C#
- BeltScript - C++

### Current status:
- Game - prototype 
- BeltScript - alpha version
