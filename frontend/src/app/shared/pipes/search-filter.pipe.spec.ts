import { SearchFilterPipe } from './search-filter.pipe';

describe('SearchFilterPipe', () => {
  let pipe: SearchFilterPipe;

  beforeEach(() => {
    pipe = new SearchFilterPipe();
  });

  it('should create an instance', () => {
    expect(pipe).toBeTruthy();
  });

  it('should return all items when searchText is empty', () => {
    const items = [
      { nombre: 'Juan', edad: 25 },
      { nombre: 'María', edad: 30 }
    ];
    const result = pipe.transform(items, '', ['nombre']);
    expect(result).toEqual(items);
  });

  it('should filter items by single field', () => {
    const items = [
      { nombre: 'Juan Pérez', edad: 25 },
      { nombre: 'María González', edad: 30 },
      { nombre: 'Pedro Martínez', edad: 35 }
    ];
    const result = pipe.transform(items, 'juan', ['nombre']);
    expect(result.length).toBe(1);
    expect(result[0].nombre).toBe('Juan Pérez');
  });

  it('should filter items by multiple fields', () => {
    const items = [
      { nombre: 'Juan', identificacion: '1234567890' },
      { nombre: 'María', identificacion: '0987654321' },
      { nombre: 'Pedro', identificacion: '1111111111' }
    ];
    const result = pipe.transform(items, '123', ['nombre', 'identificacion']);
    expect(result.length).toBe(1);
    expect(result[0].nombre).toBe('Juan');
  });

  it('should be case insensitive', () => {
    const items = [
      { nombre: 'JUAN' },
      { nombre: 'maría' },
      { nombre: 'Pedro' }
    ];
    const result = pipe.transform(items, 'JUAN', ['nombre']);
    expect(result.length).toBe(1);
  });

  it('should return empty array when no matches found', () => {
    const items = [
      { nombre: 'Juan' },
      { nombre: 'María' }
    ];
    const result = pipe.transform(items, 'xyz', ['nombre']);
    expect(result.length).toBe(0);
  });

  it('should handle nested properties', () => {
    const items = [
      { cliente: { nombre: 'Juan' } },
      { cliente: { nombre: 'María' } }
    ];
    const result = pipe.transform(items, 'juan', ['cliente.nombre']);
    expect(result.length).toBe(1);
  });
});

