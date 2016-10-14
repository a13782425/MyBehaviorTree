# MyBehaviorTree
 
2016-09-15 初始版本

2016-09-15 修改编辑器显示样式

2016-09-16 增加树管理器


使用方法：树文件继承BAIBehaviorTree，实现Init方法做一些初始化工作
          BAIBehaviorTree：
                          字段：
                                AIName：当前AI的名字，暂时没用
                                AITreeText：行为树文件
                                AITreeName：树名称，一个树文件可包含多个树
                                Interval：执行间隔时间
                                IsRun:是否运行AI
                                RunEnum：在哪里运行
                                StartTime：开始时间，游戏开始后多少秒后开始运行AI，默认为0
                                MyData：AI运行是否传递的参数，可为空
                          方法：
                                Excute():手动执行AI，不推荐
                                StopAI():停止当前AI
                                StartAI():开始当前AI
          如何生成树文件：
                        行为继承BNodeAction
                        条件继承BNodeCondition
          特性：
              BField：需要显示出来的字段，不想公开又想让策划配置：ShowName：在编辑器中显示的名字
              BClass：行为树类上面作为注释可在编辑器中显示：ShowName：在编辑器中显示的名字
              BHideField：公开属性，不想让策划看到
              
Todo:增加树的全局变量
     修改Node获取Unity变量方式
     
