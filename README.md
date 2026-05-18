# Joc Incremental de Quarks

Un joc incremental on hauràs de gestionar acceleradors de quarks, boosts, singularitats i transcendències per arribar al límit matemàtic de l'univers.

---

## Mecàniques bàsiques

El joc consisteix en acumular **Quarks**, la moneda principal. Els quarks es produeixen automàticament a través dels acceleradors i s'utilitzen per comprar més acceleradors i millores.

L'objectiu final de cada run és arribar a **double.MaxValue** (~1.8×10³⁰⁸) quarks per fer una **Transcendència** i aconseguir Partícules.

---

## Acceleradors

Hi ha **8 acceleradors** en total. Cada accelerador produeix el de nivell inferior:

```
8è → 7è → 6è → 5è → 4è → 3è → 2n → 1r → Quarks
```

- El **1r accelerador** produeix Quarks directament.
- Els acceleradors del **2n al 8è** produeixen el de nivell inferior a un ritme del 11% per segon.
- Cada accelerador es desbloqueja progressivament a mesura que avances.

### Tiers

Cada 10 compres d'un accelerador, puja de **tier**, augmentant el seu multiplicador base per **x2** (o **x2.2** amb la millora corresponent). El progrés del tier es mostra com `(X/10)` al costat del comptador.

---

## Spin

El **Spin** és un divisor de la producció dels acceleradors. Com més baix és el Spin, més ràpid produeixen els acceleradors.

- El Spin comença a **1000** i es pot reduir comprant nivells de Spin.
- Cada nivell de Spin costa **x10** més que l'anterior.
- Es pot comprar el **màxim de nivells** de Spin de cop amb el botó corresponent.
- Les **Singularitats de Quarks** fan el Spin més efectiu permanentment.

---

## Quark Boost

El **Quark Boost** és el primer reset del joc. En fer-lo:

- Es **reinicia** tot el progrés d'acceleradors i Spin.
- S'obté un **multiplicador permanent** als acceleradors basat en el nombre de boosts fets.
- Es **desbloqueja** el següent accelerador.
> Amb la millora de partícula corresponent, el cost es redueix en 9.

---

## Condensació de Quarks

La **Condensació de Quarks** és una mecànica disponible a partir del **5è Boost**. Permet obtenir un multiplicador addicional a la producció del 8è accelerador.

- El multiplicador obtingut depèn de quants **ordres de magnitud** tens del 1r accelerador respecte al teu màxim anterior.
- Necessites almenys **3 ordres de magnitud** més que el teu màxim per obtenir un boost > 1.
- En fer la condensació, els acceleradors 1-7 es reinicien però el **multiplicador acumulat es manté**.

---

## Singularitat de Quarks

La **Singularitat de Quarks** és un reset més profund que el Boost. En fer-la:

- Es reinicia tot el progrés d'acceleradors, Spin i Boosts.
- S'obté un **multiplicador permanent al Spin** que el fa més efectiu.
- El cost augmenta amb cada singularitat comprada.

### Cost

```
Cost = 80 + (singularitats × 40) vuitens acceleradors
```

> Amb la millora de partícula corresponent, el cost es redueix en 9.

A partir de la **5a singularitat**, la singularitat és visible a la UI.

---

## Transcendència

La **Transcendència** és el reset final. Es produeix quan arribes a **double.MaxValue** (~1.8e308) quarks.

En transcendir:
- Es reinicia **absolutament tot** (acceleradors, Spin, Boosts, Singularitats, Quarks).
- S'obtenen **2 Partícules** (o més amb la millora corresponent).
- Les Partícules es poden gastar en **Millores de Partícula** permanents.
- Els runs futurs seran **més curts** gràcies a les millores obtingudes.

---

## Millores de Partícula

Les **Partícules** són la moneda meta del joc — no es perden mai amb cap reset. 
Es gasten en una graella de 4×4 millores on cal desbloquejar les adjacents per accedir a les noves.

A més, hi ha una **millora infinita** (×2 a totes les fonts de partícules) que es pot comprar repetidament, dobla el seu cost cada vegada.

---

## Fites (Achievements)

Hi ha **16 fites** dividides en 2 files de 8. Completar una fila sencera dona un **multiplicador x1.5** a tots els acceleradors.


---

## Sistema de Guardat

El joc té un sistema de guardat **local i al núvol**.

### Guardat local

- La partida es guarda automàticament a intervals configurables (entre 10 segons i 5 minuts).
- Les dades es guarden a:
  ```
  C:\Users\[usuari]\AppData\LocalLow\JocsDePau\PROJECTE Joc Incremental\saves\
  ```
- Es pot guardar manualment en qualsevol moment.

### Guardat al núvol

Per guardar al núvol cal **crear un compte** i **iniciar sessió**:

1. Ves a **Configuració → Crear Usuari** i introdueix un nom d'usuari i contrasenya.
2. Ves a **Configuració → Iniciar Sessió** i introdueix les teves credencials.
3. Un cop connectat, la partida es guardarà automàticament al núvol juntament amb el guardat local.

Per carregar una partida des del núvol (per exemple, en un dispositiu nou):
1. Inicia sessió amb el teu compte.
2. Prem **Carregar del núvol**.

### Opcions addicionals

**Guardar manualment**: Guarda la partida ara mateix
**Carregar localment**: Carrega la partida guardada en local
**Carregar del núvol**: Carrega la partida guardada al núvol
**Eliminar dades locals**: Esborra la partida local i comença de zero
**Tancar sessió**: Tanca la sessió (el guardat local continua actiu)

### Producció offline

El joc calcula automàticament els quarks produïts mentre no estaves jugant (fins a un màxim de 8 hores) i els afegeix en tornar a obrir el joc.

---

*Desenvolupat amb Unity · Backend: Node.js + MongoDB Atlas · Desplegat a Render*
