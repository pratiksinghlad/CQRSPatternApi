namespace CQRSPattern.Application.Mediator;

public interface IMediatorFactory
{
    IMediatorScope CreateScope();
}
