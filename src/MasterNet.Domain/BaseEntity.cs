namespace MasterNet.Domain;

/// <summary>
/// Clase base abstracta que define propiedades comunes para todas las entidades del dominio.
/// Incluye el identificador único (Id) que será la clave primaria de cada entidad.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único de la entidad. Se genera automáticamente.
    /// No es nullable porque todas las entidades deben tener un Id válido.
    /// </summary>
    public Guid Id { get; set; }
}
