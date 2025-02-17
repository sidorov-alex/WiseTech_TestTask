﻿Задача: написать алгоритм, принимающий на вход строку разделенную пробелами, и длину строки в символах.
Необходимо разбить исходный текст на строки и выравнять по указанной длине строки с помощью пробелов.
Расстояние между словами нужно заполнять равным количеством пробелов, если же это не возможно, то добавляем
еще по пробелу между словами слева направо. Если в строке помещается только 1 слово, то дополнить строку 
пробелами справа. Результат вернуть в виде единой строки, где полученный список равных по ширине строк склеен 
с помощью символа перевода строки.

Реализовать максимально производительное решение при сохранении читабельности кода, такого чтобы его можно было использовать в продакшене и поддерживать в дальнейшем.

====================================================================================================================
v1. При разбиении исходной строки на слова используется String.Split() и строки.
v2. Сделана оптимизация с использованием Span вместо String (LineAdjustmentAlgorithmFast.cs).
Результат оптимизации:
BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.2605)

| Method  | input      | Mean          | Error         | StdDev        | Gen0      | Gen1      | Gen2      | Allocated   |
|-------- |----------- |--------------:|--------------:|--------------:|----------:|----------:|----------:|------------:|
| Strings | at. [1731] |     14.497 us |     0.3909 us |     1.1466 us |    8.0872 |         - |         - |    33.04 KB |
| Spans   | at. [1731] |      5.342 us |     0.0683 us |     0.0639 us |    5.4169 |         - |         - |    22.15 KB |
| Strings | . [17310]  |    117.351 us |     1.9980 us |     2.5980 us |   62.5000 |         - |         - |   255.45 KB |
| Spans   | . [17310]  |     28.751 us |     0.1638 us |     0.1367 us |   39.9780 |         - |         - |   164.84 KB |
| Strings | [1731000]  | 42,865.243 us |   848.0218 us | 1,633.8503 us | 2700.0000 | 1700.0000 |  800.0000 | 25509.02 KB |
| Spans   | [1731000]  |  6,233.105 us |   117.7261 us |   125.9657 us | 1117.1875 |  593.7500 |  593.7500 |  16812.9 KB |
| Strings | [3462000]  | 85,333.964 us | 1,636.4342 us | 2,009.6884 us | 5000.0000 | 2833.3333 | 1000.0000 | 51016.77 KB |
| Spans   | [3462000]  |  9,082.978 us |   156.4348 us |   192.1160 us | 1656.2500 |  640.6250 |  640.6250 | 33624.02 KB |
