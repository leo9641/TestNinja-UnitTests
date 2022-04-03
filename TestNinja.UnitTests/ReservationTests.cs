using NUnit.Framework;
using TestNinja.Fundamentals;

namespace TestNinja.UnitTests
{
    [TestFixture]
    public class ReservationTests
    {
        // methodName_scenario_expectedBehaviour

        [Test]
        public void CanBeCancelledBy_AdminCancelling_ReturnTrue()
        {
            // Arrange
            var reservation = new Reservation();

            // Act
            var result = reservation.CanBeCancelledBy(new User { IsAdmin = true });

            // Assert
            Assert.IsTrue(result);
            Assert.That(result, Is.True);
            Assert.That(result == true);
        }

        [Test]
        public void CanBeCancelledBy_SameUserCancelling_ReturnTrue()
        {
            var user = new User();
            var reservation = new Reservation { MadeBy = user };

            var result = reservation.CanBeCancelledBy(user);

            Assert.IsTrue(result);
        }


        [Test]
        public void CanBeCancelledBy_AnotherUserCancelling_ReturnFalse()
        {
            var reservation = new Reservation { MadeBy = new User() };

            var result = reservation.CanBeCancelledBy(new User());

            Assert.IsFalse(result);
        }
    }
}