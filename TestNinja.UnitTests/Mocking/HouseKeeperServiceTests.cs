using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TestNinja.Mocking;

namespace TestNinja.UnitTests.Mocking
{
    [TestFixture]
    public class HouseKeeperServiceTests
    {
        private Mock<IStatementGenerator> _statementGenerator;
        private Mock<IEmailSender> _emailSender;
        private Mock<IXtraMessageBox> _messageBox;
        private HouseKeeperService _service;
        private DateTime _statementDate = new DateTime(2017, 1, 1);
        private HouseKeeper _houseKeeper;
        private readonly string _filename = "filename";

        [SetUp]
        public void SetUp()
        {
            _houseKeeper = new HouseKeeper { Email = "a", FullName = "b", Oid = 1, StatementEmailBody = "c" };

            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.Query<HouseKeeper>()).Returns(new List<HouseKeeper>
            {
                _houseKeeper,
            }.AsQueryable());

            _statementGenerator = new Mock<IStatementGenerator>();
            _emailSender = new Mock<IEmailSender>();
            _messageBox = new Mock<IXtraMessageBox>();

            _service = new HouseKeeperService(unitOfWork.Object, _statementGenerator.Object, _emailSender.Object, _messageBox.Object);
        }

        [Test]
        public void SendStatementEmails_WhenCalled_GenerateStatement()
        {
            _service.SendStatementEmails(_statementDate);

            _statementGenerator.Verify(x => x.SaveStatement(_houseKeeper.Oid, _houseKeeper.FullName, _statementDate));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void SendStatementEmails_HouseKeeperEmailIsNullOrEmptyOrWhiteSpace_ShouldNotGenerateStatement(string email)
        {
            _houseKeeper.Email = email;

            _service.SendStatementEmails(_statementDate);

            _statementGenerator.Verify(x =>
                x.SaveStatement(_houseKeeper.Oid, _houseKeeper.FullName, _statementDate),
                Times.Never);
        }

        [Test]
        public void SendStatementEmails_WhenCalled_EmailTheStatement()
        {
            _statementGenerator
                .Setup(x => x.SaveStatement(_houseKeeper.Oid, _houseKeeper.FullName, _statementDate))
                .Returns(_filename);

            _service.SendStatementEmails(_statementDate);

            _emailSender.Verify(x => 
                x.EmailFile(_houseKeeper.Email, _houseKeeper.StatementEmailBody, _filename, It.IsAny<string>()));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void SendStatementEmails_StatementFilanemIsNullOrEmptyOrWhiteSpace_ShouldNotEmailTheStatement(string statementFilename)
        {
            _statementGenerator
                 .Setup(x => x.SaveStatement(_houseKeeper.Oid, _houseKeeper.FullName, _statementDate))
                 .Returns(() => statementFilename);

            _service.SendStatementEmails(_statementDate);

            _emailSender.Verify(x =>
                x.EmailFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Test]
        public void SendStatementEmails_SendEmailFails_DisplayAMessageBox()
        {
            _statementGenerator
                   .Setup(x => x.SaveStatement(_houseKeeper.Oid, _houseKeeper.FullName, _statementDate))
                   .Returns(_filename);

            _emailSender
                .Setup(x => x.EmailFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws<Exception>();

            _service.SendStatementEmails(_statementDate);

            _messageBox.Verify(x => x.Show(It.IsAny<string>(), It.IsAny<string>(), MessageBoxButtons.OK));
        }
    }
}
