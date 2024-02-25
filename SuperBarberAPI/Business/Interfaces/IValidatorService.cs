namespace Business.Interfaces
{
    public interface IValidatorService
    {
        void Validate<T>(T model);
    }
}
