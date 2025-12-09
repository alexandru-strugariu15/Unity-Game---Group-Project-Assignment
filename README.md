# Unity-Game---Group-Project-Assignment

## Breakout Game

Jocul Breakout este un un clasic al genului arcade, bazat pe mecanica de distrugere a unui perete format din caramizi colorate, folosind o bila care ricoseaza intre limitele ecranului. Jucatorul controleaza o paleta amplasata in partea inferioara a scenei, pe care o poate deplasa orizontal pentru a respinge bila si a o directiona catre caramizi. La fiecare impact, o caramida este distrusa, iar jucatorul primeste puncte. Obiectivul principal este eliminarea tuturor caramizilor de pe ecran fara a pierde mingea. Daca bila cade sub paleta, jucatorul pierde o viata, iar jocul se reia din pozitia initiala. Breakout combina precizia, reflexul si planificarea unghiului de lovire, oferind o experienta de joc simpla, dar captivanta si dinamica.

Jocul este compus din 2 scene principale:
-**Editorul**, care permite crearea unei palete prin alinierea in forma de dreptunghi a unor SolidBlockuri. Paleta este transmisa automat in scena jocului.
Nava este formata din blocuri de 1x1 asezate pe o grila. Grila in care jucatorul proiecteaza nava se afla in partea din dreapta jos a ferestrei si este de dimensiuni minim 5x5. In dreapta sus se afla o bara orizontala in care se afla patrate verzi ce reprezinta numarul de blocuri pe care il mai poate aseza jucatorul pe grila pentru constructia navei spatiale (nava are maxim 10 blocuri). In coltul din dreapta sus se afla butonul pentru inceperea jocului (numit buton start).

-**Jocul propriu-zis**. Jucatorul incepe partida cu 3 vieti. De fiecare data cand bila ajunge sub paleta, o viata este pierduta, iar bila si paleta se reseteaza in pozitiile initiale. La distrugerea unei caramizi, se adauga un punct. Jocul se termina cand jucatorul pierde toate vietile sau cand sunt distruse toate caramizile de pe ecran.

### Detalii de Implementare

**Desenare scena:** Pe ecran se va desena nava cu paleta, impreuna cu un grid alcatuit din blocuri colorate care reprezinta caramizile destructibile. In partea superioara sau laterala vor fi afisate inimile care indica numarul de vieti ramase. Scorul va fi scris fie in consola, fie direct pe ecran prin intermediul componentei text renderer.

**Control paleta:** Paleta se afla in josul ecranului si se deplaseaza folosind tastele ← si →.

**Eliberarea bilei:** La inceputul jocului bila este lipita de marginea de sus a paletei. La apasarea tastei SPACE bila va fi lansata in sus pe o directie oblica (45°).

**Coliziuni** Bila este de forma circulara, iar componentele paletei vor fi aproximate cu AABB (axis-aligned bounding boxes)

* **Bila-caramida:** Se sparge caramida, iar bila ricoseaza (inversand una din componentele vitezei acesteia).
* **Bila-margini ecran:** Bila ricoseaza similar.
* **Bila-margine jos ecran:** Se pierde o viata, iar paleta si bila sunt resetate.
* **Bila-paleta:** Daca bila a lovit o margine verticala a paletei atunci ricoseaza in mod obisnuit. In schimb, pentru a permite controlul bilei de catre jucator, atunci cand bila loveste o margine orizontala a paletei, aceasta ricoseaza si isi schimba directia in functie de punctul de contact similar ca in jocul PONG de mai sus.
