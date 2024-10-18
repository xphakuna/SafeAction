# 使用方法
很简单，把SafeAction的文件拷出来，哪里想用就放在哪里。
下面是简单的接口实例，详细的可以看TestSafeAction1，里面的测试用例
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