namespace LogCast.Test.given_CountEvent.and_increased
{
    public class Context : given_CountEvent.Context
    {
        public override void Arrange()
        {
            base.Arrange();
            Sut.Increase();
        }
    }
}