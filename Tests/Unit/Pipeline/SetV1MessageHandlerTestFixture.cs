﻿using System;
using System.Collections.Generic;
using System.Net;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Objects;
using Lextm.SharpSnmpLib.Pipeline;
using Lextm.SharpSnmpLib.Security;
using Moq;
using Xunit;

namespace Lextm.SharpSnmpLib.Unit.Pipeline
{
    public class SetV1MessageHandlerTestFixture
    {
        [Fact]
        public void BadValue()
        {
            var handler = new SetV1MessageHandler();
            var mock = new Mock<ScalarObject>(new ObjectIdentifier("1.3.6.1.2.1.1.4.0"));
            mock.Setup(foo => foo.Data).Throws<Exception>();
            mock.Setup(foo => foo.MatchGet(new ObjectIdentifier("1.3.6.1.2.1.1.4.0"))).Returns(mock.Object);
            mock.SetupSet(foo => foo.Data = new Integer32(400)).Throws<ArgumentException>();
            var store = new ObjectStore();
            store.Add(mock.Object);
            var context = SnmpContextFactory.Create(
                new SetRequestMessage(
                    300,
                    VersionCode.V1,
                    new OctetString("lextm"),
                    new List<Variable>
                        {
                            new Variable(new ObjectIdentifier("1.3.6.1.2.1.1.4.0"), new Integer32(400))
                        }
                    ),
                new IPEndPoint(IPAddress.Loopback, 100),
                new UserRegistry(),
                null,
                null);
            handler.Handle(context, store);
            var badValue = (ResponseMessage)context.Response;
            Assert.Equal(ErrorCode.BadValue, badValue.ErrorStatus);
        }

        [Fact]
        public void GenError()
        {
            var handler = new SetV1MessageHandler();
            var mock = new Mock<ScalarObject>(new ObjectIdentifier("1.3.6.1.2.1.1.4.0"));
            mock.Setup(foo => foo.Data).Throws<Exception>();
            mock.Setup(foo => foo.MatchGet(new ObjectIdentifier("1.3.6.1.2.1.1.4.0"))).Returns(mock.Object);
            mock.SetupSet(foo => foo.Data = new OctetString("test")).Throws<Exception>();
            var store = new ObjectStore();
            store.Add(mock.Object);
            var context = SnmpContextFactory.Create(
                new SetRequestMessage(
                    300,
                    VersionCode.V1,
                    new OctetString("lextm"),
                    new List<Variable>
                        {
                            new Variable(new ObjectIdentifier("1.3.6.1.2.1.1.4.0"), new OctetString("test"))
                        }
                    ),
                new IPEndPoint(IPAddress.Loopback, 100),
                new UserRegistry(),
                null,
                null);
            handler.Handle(context, store);
            var genError = (ResponseMessage)context.Response;
            Assert.Equal(ErrorCode.GenError, genError.ErrorStatus);
        }

        [Fact]
        public void NoError()
        {
            var handler = new SetV1MessageHandler();
            var context = SnmpContextFactory.Create(
                new SetRequestMessage(
                    300,
                    VersionCode.V1,
                    new OctetString("lextm"),
                    new List<Variable>
                        {
                            new Variable(new ObjectIdentifier("1.3.6.1.2.1.1.4.0"), new OctetString("test"))
                        }
                    ),
                new IPEndPoint(IPAddress.Loopback, 100),
                new UserRegistry(),
                null,
                null);
            var store = new ObjectStore();
            store.Add(new SysContact());
            Assert.Throws<ArgumentNullException>(() => handler.Handle(null, null));
            Assert.Throws<ArgumentNullException>(() => handler.Handle(context, null));
            handler.Handle(context, store);
            var noerror = (ResponseMessage)context.Response;
            Assert.Equal(ErrorCode.NoError, noerror.ErrorStatus);
            Assert.Equal(new OctetString("test"), noerror.Variables()[0].Data);
        }

        [Fact]
        public void NoSuchName()
        {
            var handler = new SetV1MessageHandler();
            var context = SnmpContextFactory.Create(
                new SetRequestMessage(
                    300,
                    VersionCode.V1,
                    new OctetString("lextm"),
                    new List<Variable>
                        {
                            new Variable(new ObjectIdentifier("1.3.6.1.2.1.1.4.0"), new OctetString("test"))
                        }
                    ),
                new IPEndPoint(IPAddress.Loopback, 100),
                new UserRegistry(),
                null,
                null);
            var store = new ObjectStore();
            handler.Handle(context, store);
            var noSuchName = (ResponseMessage)context.Response;
            Assert.Equal(ErrorCode.NoSuchName, noSuchName.ErrorStatus);
        }
        
        [Fact]
        public void NoSuchName2()
        {
            var handler = new SetV1MessageHandler();
            var mock = new Mock<ScalarObject>(new ObjectIdentifier("1.3.6.1.2.1.1.4.0"));
            mock.Setup(foo => foo.Data).Throws<AccessFailureException>();
            mock.Setup(foo => foo.MatchGet(new ObjectIdentifier("1.3.6.1.2.1.1.4.0"))).Returns(mock.Object);
            mock.SetupSet(foo => foo.Data = new OctetString("test")).Throws<AccessFailureException>();
            var store = new ObjectStore();
            store.Add(mock.Object);
            var context = SnmpContextFactory.Create(
                new SetRequestMessage(
                    300,
                    VersionCode.V1,
                    new OctetString("lextm"),
                    new List<Variable>
                        {
                            new Variable(new ObjectIdentifier("1.3.6.1.2.1.1.4.0"), new OctetString("test"))
                        }
                    ),
                new IPEndPoint(IPAddress.Loopback, 100),
                new UserRegistry(),
                null,
                null);
            handler.Handle(context, store);
            var genError = (ResponseMessage)context.Response;
            Assert.Equal(ErrorCode.NoSuchName, genError.ErrorStatus);
        }
    }
}
