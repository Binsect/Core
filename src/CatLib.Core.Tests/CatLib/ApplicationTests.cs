﻿/*
 * This file is part of the CatLib package.
 *
 * (c) Yu Bin <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: http://catlib.io/
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CatLib.Tests
{
    [TestClass]
    public class ApplicationTests
    {
        public class TestBaseServiceProvider : IServiceProvider
        {
            /// <summary>
            /// 服务提供者初始化
            /// </summary>
            public void Init()
            {
                
            }

            /// <summary>
            /// 当注册服务提供者
            /// </summary>
            public void Register()
            {
                
            }
        }

        public class TestServiceProvider : IServiceProvider, IServiceProviderType
        {
            /// <summary>
            /// 提供者基础类型
            /// </summary>
            public Type BaseType
            {
                get { return typeof(TestBaseServiceProvider); }
            }

            /// <summary>
            /// 服务提供者初始化
            /// </summary>
            public void Init()
            {
                throw new RuntimeException("TestServiceProvider");
            }

            /// <summary>
            /// 当注册服务提供者
            /// </summary>
            public void Register()
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(RuntimeException))]
        public void RepeatInitTest()
        {
            var app = MakeApplication();

            app.Init();
        }

        [TestMethod]
        public void TestBaseTypeProvider()
        {
            var app = new Application();
            app.Bootstrap();
            app.Register(new TestServiceProvider());

            RuntimeException ex = null;
            try
            {
                app.Init();
            }
            catch (RuntimeException e)
            {
                ex = e;
            }

            Assert.AreNotEqual(null, ex);
            Assert.AreEqual("TestServiceProvider", ex.Message);
        }

        /// <summary>
        /// 未经引导的初始化
        /// </summary>
        [TestMethod]
        public void NoBootstrapInit()
        {
            var app = new Application();

            ExceptionAssert.Throws<RuntimeException>(() =>
            {
                app.Init();
            });
        }

        public class StopBootstrap : IBootstrap
        {
            public string value = string.Empty;
            public bool stop = false;
            public void Bootstrap()
            {
                value = "bootstrap";
            }
        }

        [TestMethod]
        public void TestStopBootstrap()
        {
            var bootstrapStopped = new StopBootstrap() { stop = true };
            var bootstrapNotStopped = new StopBootstrap();
            var application = Application.New();
            application.Listen<IBootstrap, object>(ApplicationEvents.Bootstrapping, (b) =>
              {
                  if (((StopBootstrap)b).stop)
                  {
                      return false;
                  }
                  return null;
              });
            application.Bootstrap(bootstrapStopped, bootstrapNotStopped);
            Assert.AreEqual(string.Empty, bootstrapStopped.value);
            Assert.AreEqual("bootstrap", bootstrapNotStopped.value);
        }

        public class StopProvider : IServiceProvider
        {
            public string value = string.Empty;
            public bool stop = false;
            public void Register()
            {
                value = "register";
            }
            public void Init()
            {
            }
        }

        [TestMethod]
        public void TestStopRegisterProvider()
        {
            var providerStopped = new StopProvider() { stop = true };
            var providerNotStopped = new StopProvider();

            var application = Application.New();
            application.Listen<IServiceProvider, object>(ApplicationEvents.OnRegisterProvider, (b) =>
            {
                if (((StopProvider)b).stop)
                {
                    return false;
                }
                return null;
            });
            application.Register(providerStopped);
            application.Register(providerNotStopped);
            Assert.AreEqual(string.Empty, providerStopped.value);
            Assert.AreEqual("register", providerNotStopped.value);
        }

        [TestMethod]
        [ExpectedException(typeof(RuntimeException))]
        public void TestInitingRegisterProvider()
        {
            var application = Application.New();
            application.Register(new StopProvider());
            application.On<IServiceProvider>(ApplicationEvents.OnProviderInit, (b) =>
            {
                application.Register(new TestServiceProvider());
            });
            application.Bootstrap();
            application.Init();
        }

        [TestMethod]
        [ExpectedException(typeof(RuntimeException))]
        public void TestTerminateRegisterProvider()
        {
            var application = Application.New();
            application.Register(new StopProvider());
            application.On(ApplicationEvents.OnTerminate, () =>
            {
                application.Register(new TestServiceProvider());
            });
            application.Bootstrap();
            application.Init();
            application.Terminate();
        }

        /// <summary>
        /// 测试终止程序
        /// </summary>
        [TestMethod]
        public void TestTerminate()
        {
            var app = new Application();
            var oldApp = App.Handler;
            var num = 0;
            oldApp.On(ApplicationEvents.OnTerminate, () =>
            {
                Assert.AreEqual(0, num++);
            });
            oldApp.On(ApplicationEvents.OnTerminated, () =>
            {
                Assert.AreEqual(1, num++);
            });
            App.Terminate();
            Assert.AreNotEqual(oldApp, App.Handler);
            Assert.AreEqual(2, num);
        }

        /// <summary>
        /// 获取版本号测试
        /// </summary>
        [TestMethod]
        public void GetVersionTest()
        {
            var app = MakeApplication();
            Assert.AreNotEqual(string.Empty, app.Version);
        }

        [TestMethod]
        public void MakeAssemblyClass()
        {
            var app = new Application();
            var sortSet = app.Make<SortSet<string, string>>();

            Assert.AreNotEqual(null, sortSet);
        }

        [TestMethod]
        public void TestOn()
        {
            var app = new Application();
            ExceptionAssert.DoesNotThrow(() =>
            {
                app.On("hello", () => { });
            });
        }

        /// <summary>
        /// 获取当前启动流程
        /// </summary>
        [TestMethod]
        public void GetCurrentProcess()
        {
            var app = MakeApplication();
            Assert.AreEqual(Application.StartProcess.Running, app.Process);
        }

        [TestMethod]
        public void TestDebugLevel()
        {
            App.DebugLevel = DebugLevels.Development;
            Assert.AreEqual(DebugLevels.Development, App.DebugLevel);
        }

        /// <summary>
        /// 重复的引导测试
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(RuntimeException))]
        public void RepeatBootstrap()
        {
            var app = new Application();
            app.Bootstrap();
            app.Init();
            app.Bootstrap();
        }

        /// <summary>
        /// 注册非法类型测试
        /// </summary>
        [TestMethod]
        public void RegisteredIllegalType()
        {
            var app = new Application();
            app.Bootstrap();
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                app.Register(null);
            });
        }

        /// <summary>
        /// 重复的注册
        /// </summary>
        [TestMethod]
        public void RepeatRegister()
        {
            var app = MakeApplication();
            app.Register(new ProviderTest1());

            ExceptionAssert.Throws<RuntimeException>(() =>
            {
                app.Register(new ProviderTest1());
            });
        }

        /// <summary>
        /// 获取运行时唯一Id
        /// </summary>
        [TestMethod]
        public void GetRuntimeId()
        {
            var app = MakeApplication();
            Assert.AreNotEqual(app.GetRuntimeId(), app.GetRuntimeId());
        }

        private static bool prioritiesTest;

        private class ProviderTest1 : IServiceProvider
        {
            [Priority(10)]
            public void Init()
            {
                prioritiesTest = true;
            }

            public void Register()
            {

            }
        }

        [Priority(5)]
        private class ProviderTest2 : IServiceProvider
        {
            public void Init()
            {
                prioritiesTest = false;
            }

            public void Register()
            {

            }
        }

        /// <summary>
        /// 优先级测试
        /// </summary>
        [TestMethod]
        public void ProvidersPrioritiesTest()
        {
            var app = new Application();
            app.OnFindType((t) =>
            {
                return Type.GetType(t);
            });
            app.Bootstrap();
            App.Register(new ProviderTest1());
            App.Register(new ProviderTest2());
            app.Init();
            Assert.AreEqual(true, prioritiesTest);
        }

        /// <summary>
        /// 无效的引导
        /// </summary>
        [TestMethod]
        public void IllegalBootstrap()
        {
            var app = new Application();
            ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                app.Bootstrap(null);
                app.Init();
            });
        }

        /// <summary>
        /// 初始化后再注册
        /// </summary>
        [TestMethod]
        public void InitedAfterRegister()
        {
            prioritiesTest = true;
            var app = new Application();
            app.OnFindType((t) =>
            {
                return Type.GetType(t);
            });
            app.Bootstrap();
            App.Register(new ProviderTest1());
            app.Init();

            App.Register(new ProviderTest2());
            Assert.AreEqual(false, prioritiesTest);
        }

        [TestMethod]
        public void TestRepeatRegister()
        {
            var app = new Application();
            app.OnFindType((t) =>
            {
                return Type.GetType(t);
            });

            app.Bootstrap();
            app.Register(new ProviderTest1());

            ExceptionAssert.Throws<RuntimeException>(() =>
            {
                app.Register(new ProviderTest1());
            });
        }

        [TestMethod]
        public void TestOnDispatcher()
        {
            var app = MakeApplication();

            app.Listen("testevent", (object payload) =>
            {
                Assert.AreEqual("abc", payload);
                return 123;
            });

            var result = app.TriggerHalt("testevent", "abc");
            Assert.AreEqual(123, result);
        }

        [TestMethod]
        public void TestIsMainThread()
        {
            var app = MakeApplication();
            Assert.AreEqual(true, app.IsMainThread);
        }

        public class TestRegisterProcessMakeServiceProvider : ServiceProvider
        {
            public override void Register()
            {
                App.Make<object>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CodeStandardException))]
        public void TestRegisterProcessMake()
        {
            var app = MakeApplication();
            app.Register(new TestRegisterProcessMakeServiceProvider());
        }

        private Application MakeApplication()
        {
            var app = new Application();
            app.OnFindType((t) =>
            {
                return Type.GetType(t);
            });
            app.Bootstrap(new BootstrapClass());
            app.Init();
            return app;
        }

        private class BootstrapClass : IBootstrap
        {
            public void Bootstrap()
            {
            }
        }
    }
}
