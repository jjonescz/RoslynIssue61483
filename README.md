```ps1
dotnet run -c Release -f net8.0 --filter '**'
```

## Results

```log
BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1265/22H2/2022Update/SunValley2), VM=Hyper-V
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=8.0.100-preview.1.23115.2
  [Host]     : .NET 8.0.0 (8.0.23.11008), X64 RyuJIT AVX2
  Job-GWYXTQ : .NET 7.0.3 (7.0.323.6910), X64 RyuJIT AVX2
  Job-ZKBEJK : .NET 7.0.3 (7.0.323.6910), X64 RyuJIT AVX2
  Job-FEGVAA : .NET 8.0.0 (8.0.23.11008), X64 RyuJIT AVX2
  Job-PVAAGM : .NET 8.0.0 (8.0.23.11008), X64 RyuJIT AVX2
```

|   Method |  Runtime |  BuildConfiguration |      Mean | Ratio | Code Size |
|--------- |--------- |-------------------- |----------:|------:|----------:|
| Compare1 | .NET 7.0 |             Release | 0.6837 ns |  1.05 |      25 B |
| Compare1 | .NET 7.0 | ReleaseCustomRoslyn | 0.6531 ns |  1.05 |      25 B |
| Compare1 | .NET 8.0 |             Release | 0.6459 ns |  1.00 |      25 B |
| Compare1 | .NET 8.0 | ReleaseCustomRoslyn | 0.6447 ns |  1.00 |      25 B |
|          |          |                     |           |       |           |
| Compare2 | .NET 7.0 |             Release | 0.6012 ns |  0.71 |      31 B |
| Compare2 | .NET 7.0 | ReleaseCustomRoslyn | 0.2925 ns |  0.35 |      20 B |
| Compare2 | .NET 8.0 |             Release | 0.8460 ns |  1.00 |      31 B |
| Compare2 | .NET 8.0 | ReleaseCustomRoslyn | 0.1157 ns |  0.18 |      20 B |
|          |          |                     |           |       |           |
| Compare3 | .NET 7.0 |             Release | 0.7485 ns |  0.85 |      32 B |
| Compare3 | .NET 7.0 | ReleaseCustomRoslyn | 0.5573 ns |  0.64 |      37 B |
| Compare3 | .NET 8.0 |             Release | 0.8691 ns |  1.00 |      32 B |
| Compare3 | .NET 8.0 | ReleaseCustomRoslyn | 0.6283 ns |  0.72 |      37 B |

```cs
public int Compare1(int x, int y)
{
    if (x < y) return -1;
    if (x > y) return 1;
    return 0;
}

public int Compare2(int x, int y)
{
    int tmp1 = (x > y) ? 1 : 0;
    int tmp2 = (x < y) ? 1 : 0;
    return tmp1 - tmp2;
}

public int Compare3(char c)
{
    return (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z') ? 1 : 0;
}
```

### Compare2 Release
```assembly
; Program.Compare2(Int32, Int32)
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

### Compare2 ReleaseCustomRoslyn
```assembly
; Program.Compare2(Int32, Int32)
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

### Compare3 Release
```assembly
; Program.Compare3(Char)
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

### Compare3 ReleaseCustomRoslyn
```assembly
; Program.Compare3(Char)
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
