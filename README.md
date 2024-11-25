Esta fase del proyecto plantea comparar los siguientes algoritmos/métodos:
 - Algoritmo de Thompson
 - Método de Brzozowski

# Algoritmo de Thompson
El algoritmo de Thompson convierte una expresion regular (RE) a su autómata finito no determinista (NDFA) equivalente y luego hay que aplicar el *Algoritmo de subconjuntos* 
para convertir ese NDFA a su autómata finito determinista (DFA) equivalente.

# Método de Thompson
Este método convierte una expresión regular (RE) a su autómata finito determinista (DFA) equivalente sin tener que hacer otra conversión.

# Resultados
El proyecto plantea mostrar un benchmark de ambos algoritmos:
 - Benchmark de contrucción de los automátas
 - Benchmark de la evalucación de cadenas
