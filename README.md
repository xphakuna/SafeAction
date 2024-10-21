# 使用方法
很简单，把SafeAction的文件拷出来，哪里想用就放在哪里。
下面是简单的接口用例，详细的可以看TestSafeAction1里面的测试用例
```cshar
var sa = new SafeAction.SafeAction();
var act1 = () => { n1++; };
// add action
sa.AddAction(null, act1);
// remove action
sa.RemoveAction(null, act1);
// invoke
sa.Invoke();
```
# 解决痛点
## 解决痛点之action的exception
```cshar
System.Action SystemAction = null;
SystemAction += act1;
SystemAction += act2;
SystemAction += act3;
SystemAction.Invoke(); // act2 have exception, act3 will not excute
```
上面的代码，如果act2抛出异常，act3不会执行。用safeAction可以解决这个问题
## 解决痛点之action忘记remove会有大问题
```cshar
// open some ui
System.Action SystemAction = null;
SystemAction += act1;
```
上面的代码，打开界面的时候增加action，如果关闭界面没有remove，会有大问题。
```cshar
var sa = new SafeAction.SafeAction();
var act1 = () => { n1++; };
// add action
sa.AddAction(yourUIObj, act1);
SafeAction.S_addCheckValideFunc(typeof(yourUIType), (obj)=>{
    return (yourUIType)obj.IsRemoved();
})
```
上面的代码是safeAction的版本，AddAction多传入一个yourUIObj，加入一个检查界面是否关闭的函数。（这个检查函数可以只初始化一次，yourUIType可以是UI的基类，就不用每个类型都加一个了）
## 解决痛点之重复加入action，会执行多次
```cshar
System.Action SystemAction = null;
SystemAction += act1;
SystemAction += act1;
SystemAction.Invoke();
```
上面的代码，如果act1会执行两遍。用safeAction可以解决这个问题