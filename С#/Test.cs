long n = 1000000;
long s = 0;
long i = 0;
while (i < n)
{
    s = s + (i * i - i) / (i + 1);
    i = i + 1;
}
print(s);