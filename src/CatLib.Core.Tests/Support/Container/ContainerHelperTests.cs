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
    public class ContainerHelperTests
    {
        /// <summary>
        /// 生成服务和转为目标
        /// </summary>
        [TestMethod]
        public void MakeTConvert()
        {
            var container = MakeContainer();
            var obj = container.Make<ContainerHelperTests>("ContainerHelperTests");
            Assert.AreSame(this, obj);
        }

        /// <summary>
        /// 生成服务和转为目标
        /// </summary>
        [TestMethod]
        public void MakeTService()
        {
            var container = MakeContainer();
            var obj = container.Make<ContainerHelperTests>();
            Assert.AreSame(this, obj);
        }

        [TestMethod]
        public void MakeTypeService()
        {
            var container = MakeContainer();
            var obj = container.Make(typeof(ContainerHelperTests));
            Assert.AreSame(this, obj);
        }

        /// <summary>
        /// 以单例形式绑定
        /// </summary>
        [TestMethod]
        public void BindSingleton()
        {
            var container = MakeContainer();
            container.Singleton("BindSingleton", (c, param) =>
            {
                return new object();
            });
            var obj = container.Make("BindSingleton");
            Assert.AreSame(obj, container.Make("BindSingleton"));
        }

        public class ContainerHelperTestClass
        {

        }

        public class TestClassService
        {

        }

        /// <summary>
        /// 以单列形式绑定
        /// </summary>
        [TestMethod]
        public void BindSingletonTServiceTConcrete()
        {
            var container = MakeContainer();
            container.Singleton<TestClassService, ContainerHelperTestClass>();
            var obj = container.Make(container.Type2Service(typeof(ContainerHelperTestClass)));
            var obj2 = container.Make(container.Type2Service(typeof(TestClassService)));

            Assert.AreSame(obj, obj2);
        }

        /// <summary>
        /// 以单列形式绑定
        /// </summary>
        [TestMethod]
        public void SingletonTService()
        {
            var container = MakeContainer();
            container.Singleton<TestClassService>((c, p) =>
            {
                return new object();
            });
            var obj = container.Make(container.Type2Service(typeof(TestClassService)));
            var obj2 = container.Make(container.Type2Service(typeof(TestClassService)));

            Assert.AreSame(obj, obj2);
        }

        [TestMethod]
        public void TestInstance()
        {
            var container = MakeContainer();
            var obj = new TestClassService();
            container.Instance<TestClassService>(obj);

            Assert.AreSame(obj, container.Make<TestClassService>());
        }

        [TestMethod]
        public void TestRelease()
        {
            var container = MakeContainer();
            var obj = new TestClassService();
            container.Instance<TestClassService>(obj);
            container.OnFindType((str) =>
            {
                return Type.GetType(str);
            });

            Assert.AreSame(obj, container.Make<TestClassService>());
            container.Release<TestClassService>();
            // 因为被释放后容器会容器会自动推测出所需类的实例
            Assert.AreSame(obj.GetType(), container.Make<TestClassService>().GetType());
        }

        [TestMethod]
        public void TestBindIf()
        {
            var app = new Application();
            IBindData bindData;
            Assert.AreEqual(true, App.BindIf("TestBind", (c, p) => 1, out bindData));
            Assert.AreEqual(false, App.BindIf("TestBind", (c, p) => 2, out bindData));
            Assert.AreEqual(1, app["TestBind"]);

            Assert.AreEqual(true, App.BindIf<object>(out bindData));
            Assert.AreEqual(typeof(object), app.Make<object>().GetType());

            Assert.AreEqual(true, App.BindIf<long>((c,p) => 100, out bindData));
            Assert.AreEqual(true, App.BindIf<int>(() => 100, out bindData));
            Assert.AreEqual(100, App.Make<int>());
            Assert.AreEqual(100, App.Make<int>()); // double get check
            Assert.AreEqual(true, App.BindIf<double, float>(out bindData));
            Assert.AreEqual(false, App.BindIf<double, float>(out bindData));

            Assert.AreEqual(typeof(double), App.Make<double>(App.Type2Service(typeof(float))).GetType());
        }

        [TestMethod]
        public void TestSingletonIf()
        {
            var testObject = new object();
            var testObject2 = new object();
            var app = new Application();
            IBindData bindData;
            Assert.AreEqual(true, App.SingletonIf("TestBind", (c, p) => new object(), out bindData));

            var makeObject = app["TestBind"];
            Assert.AreEqual(false, App.SingletonIf("TestBind", (c, p) => testObject2, out bindData));
            Assert.AreSame(testObject.GetType(), makeObject.GetType());
            Assert.AreSame(makeObject, app["TestBind"]);

            Assert.AreEqual(true, App.SingletonIf<object>(out bindData));
            Assert.AreEqual(typeof(object), app.Make<object>().GetType());

            Assert.AreEqual(true, App.SingletonIf<long>((c, p) => 100, out bindData));
            Assert.AreEqual(true, App.SingletonIf<int>(() => 100, out bindData));
            Assert.AreEqual(100, App.Make<int>());
            Assert.AreEqual(true, App.SingletonIf<double, float>(out bindData));
            Assert.AreEqual(false, App.SingletonIf<double, float>(out bindData));

            Assert.AreEqual(typeof(double), App.Make<double>(App.Type2Service(typeof(float))).GetType());
        }

        [TestMethod]
        public void TestGetBind()
        {
            var container = new Container();
            var bind = container.Bind<IApplication>(() => "helloworld");

            Assert.AreEqual("helloworld", container.Make(container.Type2Service<IApplication>()));
            Assert.AreEqual(true, container.HasBind<IApplication>());
            Assert.AreSame(bind, container.GetBind<IApplication>());
        }

        [TestMethod]
        public void TestCanMake()
        {
            var container = new Container();

            Assert.AreEqual(false, container.CanMake<IApplication>());
            container.Bind<IApplication>(() => "helloworld");
            Assert.AreEqual(true, container.CanMake<IApplication>());
        }

        [TestMethod]
        public void TestIsStatic()
        {
            var container = new Container();
            container.Bind<IApplication>(() => "helloworld");
            Assert.AreEqual(false, container.IsStatic<IApplication>());
            container.Unbind<IApplication>();
            container.Singleton<IApplication>(() => "helloworld");
            Assert.AreEqual("helloworld", container.Make(container.Type2Service<IApplication>()));
            Assert.AreEqual(true, container.IsStatic<IApplication>());
        }

        [TestMethod]
        public void TestIsAlias()
        {
            var container = new Container();
            container.Bind<IApplication>(() => "helloworld").Alias<IContainer>();
            Assert.AreEqual(false, container.IsAlias<IApplication>());
            Assert.AreEqual(true, container.IsAlias<IContainer>());
        }

        [TestMethod]
        public void TestGetService()
        {
            var container = new Container();
            Assert.AreEqual(container.Type2Service(typeof(string)), container.Type2Service<string>());
        }

        public class TestWatchCLass
        {
            public int value;

            public IContainer container;

            public void OnChange(int instance, IContainer container)
            {
                value = instance;
                this.container = container;
            }
        }

        public interface IWatchTest
        {
            int getValue();
        }

        public class TestData : IWatchTest
        {
            private int val;
            public TestData(int val)
            {
                this.val = val;
            }
            public int getValue()
            {
                return val;
            }
        }

        [TestMethod]
        public void TestWatch()
        {
            var container = new Container();
            container.Instance<IContainer>(container);

            var cls = new TestWatchCLass();
            container.Watch<IWatchTest>(cls, "OnChange");
            container.Instance<IWatchTest>(100);
            container.Instance<IWatchTest>(200);

            Assert.AreEqual(200, cls.value);
            Assert.AreSame(container, cls.container);
        }

        [TestMethod]
        public void TestWatchLambda()
        {
            var container = new Container();
            container.Instance<IContainer>(container);

            var isCall = false;
            container.Watch<IWatchTest>((val) =>
            {
                isCall = true;
                Assert.AreEqual(200, val.getValue());
            });
            container.Instance<IWatchTest>(new TestData(100));
            container.Instance<IWatchTest>(new TestData(200));

            Assert.AreEqual(true, isCall);
            Assert.AreEqual(true, isCall);
        }

        [TestMethod]
        public void TestWatchLambdaNoParam()
        {
            var container = new Container();
            container.Instance<IContainer>(container);

            var isCall = false;
            container.Watch<IWatchTest>(() =>
            {
                isCall = true;
            });
            container.Instance<IWatchTest>(100);
            container.Instance<IWatchTest>(200);

            Assert.AreEqual(true, isCall);
        }

        [TestMethod]
        public void TestReleaseWithObject()
        {
            var container = new Container();
            container.Instance<string>("abc");
            container.Instance<int>(10);
            object[] data = null;
            Assert.AreEqual(true, container.Release(ref data));

            data = new object[] {"abc", 10};
            Assert.AreEqual(true, container.Release(ref data));
            Assert.AreEqual(true, data.Length == 0);

            data = new object[] { "abc", 10, 998 };
            Assert.AreEqual(false, container.Release(ref data));
            Assert.AreEqual(true, data.Length == 3);

            container.Instance<int>(10);
            data = new object[] { "abc", 10, 998 };
            Assert.AreEqual(false, container.Release(ref data, false));
            Assert.AreEqual(true, data.Length == 2);
            Assert.AreEqual("abc", data[0]);
            Assert.AreEqual(998, data[1]);

            container.Instance<string>("abc");
            data = new object[] { 10 , "abc", 998 };
            Assert.AreEqual(false, container.Release(ref data));
            Assert.AreEqual(true, data.Length == 2);
            Assert.AreEqual(10, data[0]);
            Assert.AreEqual(998, data[1]);
        }

        /// <summary>
        /// 生成容器
        /// </summary>
        /// <returns>容器</returns>
        private Container MakeContainer()
        {
            var container = new Container();
            container.Instance("ContainerHelperTests", this);
            container.Instance(container.Type2Service(typeof(ContainerHelperTests)), this);
            return container;
        }
    }
}
