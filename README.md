[![N|Solid](https://github.com/blowin/Apophis/blob/master/icon.png?raw=true)](https://vk.com/achillbad)

In example use simple extension 'Print' source code:
```c#
public static void Print(this object obj, string end = "\n")
{
    Console.Write(obj);
    Console.Write(end);
}
```

### Option example:

Format
```c#
Optional.Some(2).Print(); // Some(2)
            
"Hello Apophis".ToOption().ToOption().ToOption().Print(); // Some(Some(Some(Hello Apophis)))
Optional.None<int>().Print(); // None
```

Create way
```c#
Option<string> optionNone = Optional.None<string>();
Option<string> optionNone2 = null;
Option<string> optionNone3 = new Option<string>();

Option<int> option = 20;
Option<int> option2 = Optional.Some(20);
Option<int> option3 = 20.ToOption();
Option<int> option4 = new Option<int>(20);
```

Pattern matching for optional:
```c#
Option<string> none = null;

none.Match( // Option is empty
    some: s => $"Hold value is: {s}".Print(), 
    none: () => "Option is empty".Print()
);

none.Match(() => "Empty".Print()); // print Empty
none.Match(s => $"Has {s}".Print()); // not print

string returnMath = none.Match(
    s => s, 
    () => "default value"
); // return "default value"

var someOpt = "Hello".ToOption();

string returnMath2 = someOpt.Match(
    s => s,
    () => "world"
); // return "Hello"
```

Operators example:
```c#
var option1 = 2.ToOption();

var option2 = Optional.Some(System.Math.PI).Map(d => d * System.Math.E);

var result = option1
    .Map(i => i * 200) // 2 * 200 == 400
    .Filter(i => i > 200) // 400 > 200 
    .FlatMap(i => option2.Map(d => d * i)) // unbox value from option2
    .FoldLeft(23.2, (d, d1) => d / d1); // return 23.2 if empty, or holdValue / 23.2

result.Print();
```
