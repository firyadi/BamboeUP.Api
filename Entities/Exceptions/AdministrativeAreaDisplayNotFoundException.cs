namespace Entities.Exceptions
{
    public sealed class AdministrativeAreaDisplayNotFoundException : NotFoundException
    {
        public AdministrativeAreaDisplayNotFoundException(string postalCode)
            : base($"The administrative area with PostalCode: {postalCode} doesn't exist in the database.")
        {
        }
    }
}
