```ps1
dotnet run -c Release --filter '**'
```

## Results

```log
BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1265/22H2/2022Update/SunValley2)
11th Gen Intel Core i7-11800H 2.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK=8.0.100-preview.1.23115.2
  [Host]   : .NET 7.0.3 (7.0.323.6910), X64 RyuJIT AVX2
  ShortRun : .NET 7.0.3 (7.0.323.6910), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  
```

|   Method |  Runtime |  BuildConfiguration |      Mean |  Ratio | Code Size |
|--------- |--------- |-------------------- |----------:|-------:|----------:|
| Compare2 | .NET 7.0 |             Release | 0.6852 ns |        |      31 B |
| Compare2 | .NET 7.0 | ReleaseCustomRoslyn | 0.0239 ns |        |      20 B |
| Compare2 | .NET 8.0 |             Release | 0.5440 ns |   1.00 |      31 B |
| Compare2 | .NET 8.0 | ReleaseCustomRoslyn | 0.0155 ns |   0.03 |      20 B |
|          |          |                     |           |        |           |
| Compare3 | .NET 7.0 |             Release | 0.3414 ns |        |      32 B |
| Compare3 | .NET 7.0 | ReleaseCustomRoslyn | 0.2340 ns |        |      37 B |
| Compare3 | .NET 8.0 |             Release | 0.2393 ns |   1.00 |      32 B |
| Compare3 | .NET 8.0 | ReleaseCustomRoslyn | 0.2657 ns |   1.13 |      37 B |

```cs
[Benchmark, Arguments(1, 2)]
public int Compare2(int x, int y)
{
    int tmp1 = (x > y) ? 1 : 0;
    int tmp2 = (x < y) ? 1 : 0;
    return tmp1 - tmp2;
}

[Benchmark, Arguments('A')]
public int Compare3(char c)
{
    return (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z') ? 1 : 0;
}
```

### Compare2 Release .NET 8
```assembly
       cmp       edx,r8d
       jg        short M00_L00
       xor       eax,eax
       jmp       short M00_L01
M00_L00:
       mov       eax,1
M00_L01:
       cmp       edx,r8d
       jl        short M00_L02
       xor       edx,edx
       jmp       short M00_L03
M00_L02:
       mov       edx,1
M00_L03:
       sub       eax,edx
       ret
; Total bytes of code 31
```

### Compare2 ReleaseCustomRoslyn .NET 8
```assembly
       xor       eax,eax
       cmp       edx,r8d
       setg      al
       cmp       edx,r8d
       setl      dl
       movzx     edx,dl
       sub       eax,edx
       ret
; Total bytes of code 20
```

### Compare3 Release .NET 8
```assembly
       movzx     eax,dx
       cmp       eax,41
       jl        short M00_L00
       cmp       eax,5A
       jle       short M00_L02
M00_L00:
       cmp       eax,61
       jl        short M00_L01
       cmp       eax,7A
       jle       short M00_L02
M00_L01:
       xor       eax,eax
       ret
M00_L02:
       mov       eax,1
       ret
; Total bytes of code 32
```

### Compare3 ReleaseCustomRoslyn .NET 8
```assembly
       movzx     eax,dx
       cmp       eax,41
       jl        short M00_L00
       cmp       eax,5A
       jle       short M00_L02
M00_L00:
       cmp       eax,61
       jl        short M00_L01
       cmp       eax,7A
       setle     al
       movzx     eax,al
       ret
M00_L01:
       xor       eax,eax
       ret
M00_L02:
       mov       eax,1
       ret
; Total bytes of code 37
```
