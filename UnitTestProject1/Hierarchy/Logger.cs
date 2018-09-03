using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using UnitTestProject1.Appender;
using System.Collections;

namespace UnitTestProject1.Hierarchy
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void TestHierarchy()
        {
            log4net.Repository.Hierarchy.Hierarchy h = new log4net.Repository.Hierarchy.Hierarchy();
            h.Root.Level = Level.Error;

            //获得一个记录器（如果不存在，则创建并返回）
            Logger a0 = (Logger)h.GetLogger("a");
            Assert.AreEqual("a", a0.Name);
            Assert.IsNull(a0.Level);
            Assert.AreSame(Level.Error, a0.EffectiveLevel);

            //获得之前创建的记录器
            Logger a1 = (Logger)h.GetLogger("a");
            Assert.AreSame(a0, a1);
        }

        [TestMethod]
        public void TestExists()
        {
            object a = Utils.GetLogger("a");
            object a_b = Utils.GetLogger("a.b");
            object a_b_c = Utils.GetLogger("a.b.c");
            object t;

            t = LogManager.Exists("xx");
            Assert.IsNull(t);
            t = LogManager.Exists("a");
            Assert.AreSame(a, t);
            t = LogManager.Exists("a.b");
            Assert.AreSame(a_b, t);
            t = LogManager.Exists("a.b.c");
            Assert.AreSame(a_b_c, t);
        }

        [TestMethod]
        public void TestAppender1()
        {
            Logger log = (Logger)Utils.GetLogger("test").Logger;
            CountingAppender a1 = new CountingAppender();
            a1.Name = "testAppender1";
            log.AddAppender(a1);

            Assert.IsTrue(log.Appenders.Count > 0);

            var enumAppenders = ((IEnumerable)log.Appenders).GetEnumerator();
            Assert.IsTrue(enumAppenders.MoveNext());
            var a2 = (CountingAppender)enumAppenders.Current;
            Assert.AreEqual(a1, a2);
        }

        [TestMethod]
        public void TestAdditivity()
        {
            Logger a = (Logger)Utils.GetLogger("a").Logger;

            var ca = new CountingAppender();

            a.AddAppender(ca);
            a.Repository.Configured = true;//设置了之后，Level才起作用。

            Assert.AreEqual(ca.Counter, 0);

            a.Log(Level.Debug, "test debug log", null);
            Assert.AreEqual(ca.Counter, 1);

            a.Log(Level.Info, "test info log", null);
            Assert.AreEqual(ca.Counter, 2);

            a.Log(Level.Warn, "test warn log", null);
            Assert.AreEqual(ca.Counter, 3);

            a.Log(Level.Error, "test error log", null);
            Assert.AreEqual(ca.Counter, 4);
        }

        [TestMethod]
        public void TestAdditivity2()
        {
            /*
                记录器的层次Logger Hierarchy
                https://blog.csdn.net/redez/article/details/518834
                首先，我们先看一下何为层次，以我们最熟悉的继承为例。
                假如类B是类C的父类，类A是类C的祖先类，类D是类C的子类。
                这些类之间就构成一种层次关系。
                在这些具有层次关系的类中，子类都可继承它的父类的特征，如类B的对象能调用类A中的非private实例变量和函数；
                而类C由于继承自类B，所以类B的对象可以同时调用类A和类B中的非private实例变量和函数。

                在log4j中，处于不同层次中的Logger也具有象类这样的继承关系。
                如果一个应用中包含了上千个类，那么也几乎需要上千个Logger实例。
                如何对这上千个Logger实例进行方便地配置，就是一个很重要的问题。
                Log4J采用了一种树状的继承层次巧妙地解决了这个问题。
                在Log4J中Logger是具有层次关系的。它有一个共同的根，位于最上层，其它Logger遵循类似包的层次。

                根记录器root logger
                就象一个Java中的Object类一样，log4j中的logger层次中有一个称之为根记录器的记录器，
                其它所有的记录器都继承自这个根记录器。根记录器有两个特征：
                1) 根记录器总是存在。就像Java中的Object类一样，因为用log4j输出日志信息是通过记录器来实现的，
                   所以只要你应用了log4j，根记录器就肯定存在的。
                2) 根记录器没有名称，所以不能通过名称来取得根记录器。
                   但在Logger类中提供了getRootLogger()的方法来取得根记录器。
                
                记录器的层次
                Logger遵循类似包的层次。如
                static Logger rootLog = Logger.getRootLogger();
                static Logger log1 = Logger.getLogger("test4j");
                static Logger log2 = Logger.getLogger("test4j.test4j2");
                static Logger log3 = Logger.getLogger("test4j.test4j2.test4j2");
                那么rootLog是log2的祖先子记录器，log1是log2的父记录器，log3是log2的子记录器。
                记录器象Java中的类继承一样，子记录器可以继承父记录器的设置信息，也可以可以覆写相应的信息。
             */

            Logger a = (Logger)Utils.GetLogger("a").Logger;
            Logger ab = (Logger)Utils.GetLogger("a.b").Logger;
            Logger abc = (Logger)Utils.GetLogger("a.b.c").Logger;
            Logger x = (Logger)Utils.GetLogger("x").Logger;

            CountingAppender ca1 = new CountingAppender();
            CountingAppender ca2 = new CountingAppender();

            a.AddAppender(ca1);
            abc.AddAppender(ca2);
            a.Repository.Configured = true;

            Assert.AreEqual(ca1.Counter, 0);
            Assert.AreEqual(ca2.Counter, 0);

            ab.Log(Level.Debug, "", null);
            Assert.AreEqual(ca1.Counter, 1);
            Assert.AreEqual(ca2.Counter, 0);

            abc.Log(Level.Debug, "", null);
            Assert.AreEqual(ca1.Counter, 2);
            Assert.AreEqual(ca2.Counter, 1);

            x.Log(Level.Debug, "", null);
            Assert.AreEqual(ca1.Counter, 2);
            Assert.AreEqual(ca2.Counter, 1);
        }

    }
}
