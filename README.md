# Proyecto de Juego en Unity

## Versión de Unity
- Unity 6000.0.34f1  

## Instrucciones de compilación y ejecución
1. Clonar o descargar el repositorio.
2. Abrir la carpeta del proyecto con Unity Hub.
3. Seleccionar la versión **6000.0.34f1** (u otra compatible).
4. Presionar **Play** en el editor o generar un build con **File > Build Settings**.

## Controles
- Navegación: Mouse/Teclado (dependiendo del minijuego o interfaz).
- Interacciones principales gestionadas por `OptionController` y `QuestionController`.

## Dependencias
- No se usan dependencias externas por ahora (solo librerías estándar de Unity y C#).

## Arquitectura y decisiones técnicas
- **Controllers**: manejan lógica de interacción (ej. preguntas y opciones).
- **Data**: define estructuras de datos (`PlayerData`, `QuestionData`).
- **JsonHandlers**: lectura/escritura de datos en JSON (preguntas, ranking).
- **Managers**: controlan aspectos globales como partida (`GameManager`), puntuación, ranking y registro de jugadores.
- Se sigue una estructura modular para mantener **separación de responsabilidades** y facilitar mantenimiento.
