# Console Password Manager

## Über das Projekt
Der **Console Password Manager** ist eine einfache Konsolenanwendung in C#, die es Nutzern ermöglicht, Passwörter sicher zu speichern, zu verwalten und abzurufen. Das Projekt dient dazu, das Verständnis für Methoden, Logik und Dateioperationen in C# zu vertiefen.

## Funktionen
- **Passwort speichern**: Nutzer können Passwörter sicher abspeichern.
- **Passwort abrufen**: Gespeicherte Passwörter können abgerufen werden.
- **Passwort löschen**: Entferne gespeicherte Einträge.
- **Passwort generieren**: Nutzer können komplizierte Passwörter im Bereich zwischen 1-130 Zeichen generieren lassen.
- **Passwort suchen**: Mit der Suchfunktion macht die Konsolen App es bei grösseren Mengen an Einträgen einfacher nach einem bestimmten Passwort zu suchen.
- **Masterpasswort ändern**: Falls man das Masterpasswort ändern möchte kann man das über eine Funktion in der App erledigen.
- **Verschlüsselte Speicherung**: Passwörter werden verschlüsselt in einer Datei gespeichert.
- **Benutzerfreundliche CLI**: Einfache Bedienung über die Konsole.

## Installation Option 1
1. Stelle sicher, dass **.NET SDK 6.0 oder höher** installiert ist.
2. Klone das Repository:
   ```sh
   git clone https://github.com/ibrazqrj/ConsolePasswordManager.git
   ```
3. Wechsle in das Projektverzeichnis:
   ```sh
   cd ConsolePasswordManager
   ```
4. Baue das Projekt:
   ```sh
   dotnet build
   ```

## Nutzung
Starte die Anwendung mit:
```sh
dotnet run
```

## Installation Option 2
1. "Password Manager.zip" herunterladen, und die .exe Datei ausführen.

Folge den Anweisungen in der Konsole, um Passwörter hinzuzufügen, zu verwalten oder zu löschen.

## Sicherheit
- Die gespeicherten Passwörter sind verschlüsselt.
- Nutze eine **sicheres Masterpasswort**, um Zugriff auf gespeicherte Passwörter zu erhalten.


## Autor
[ibrazqrj](https://github.com/ibrazqrj)