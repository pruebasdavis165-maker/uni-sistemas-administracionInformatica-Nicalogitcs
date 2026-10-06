namespace NicaLogistics.Application.UseCases.Catalog;

public record ProductoDto(
    int IdProducto,
    string Nombre,
    string Descripcion,
    decimal Precio,
    int Existencias,
    bool Estado,
    int IdCategoria,
    string NombreCategoria
);
