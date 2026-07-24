# SL-CustomObjects Geliştirici Dokümantasyonu (Türkçe)

**SL-CustomObjects** projesinin resmi geliştirici dokümanına hoş geldiniz. Bu araç, *SCP: Secret Laboratory* oyunu için hazırlanan **MapEditorReborn (MER)** (LabAPI ve EXILED sürümleri) eklentisine Unity tarafında özel harita şablonları tasarlamanızı ve dışa aktarmanızı sağlar.

---

## 📖 Mimari ve Tasarım Genel Bakış

SL-CustomObjects; Unity içinde oluşturduğunuz odaları, yapıları ve etkileşimli nesne grupları sunucu tarafının okuyabileceği JSON şablonlarına dönüştürür.

```
+---------------------------------------+
|              Unity Editor             |
|    (Hiyerarşik Tasarımların Çizimi)    |
+---------------------------------------+
                    |
                    v (Compile Schematic - Şablon Derleme)
+---------------------------------------+
|         JSON Serileştirme Dosyaları    |
|    - [Ad].json (Genel Nesneler)       |
|    - [Ad]-Rigidbodies.json (Fizik)    |
|    - [Ad]-Teleports.json (Işınlanma)  |
+---------------------------------------+
                    |
                    v (Sunucuya Aktarma)
+---------------------------------------+
|          SCP: SL Oyun Sunucusu        |
|     (MapEditorReborn ile Oluşturma)   |
+---------------------------------------+
```

---

## 🛠️ Kod Yapısı ve Dosya Açıklamaları

### 1. Temel Sınıflar

#### 🔹 [Schematic.cs](file:///home/flaouve/Projects/scp%20sl/Map/Fla-SL-CustomObjects/Assets/DONT%20TOUCH/Scripts/Schematic.cs)
Her şablon yapısının kök (root) nesnesinde bulunması gereken ana bileşendir. Derleme süreçlerini yönetir.
* **`CompileSchematic()`**: Altındaki tüm `SchematicBlock` bileşenlerini tarar, nesne hiyerarşisini listeler, animasyonları AssetBundle olarak derler, fizik/ışınlanma verilerini ayırır ve JSON çıktısı üretir. Ayarlarda aktifse tüm bunları `.zip` dosyası haline getirir.
* **`Update()`**: Kök nesnenin boyutunun (scale) değiştirilmesini engeller ve klasör isminde boşluk olmamasını denetler.

#### 🔹 [SchematicBlock.cs](file:///home/flaouve/Projects/scp%20sl/Map/Fla-SL-CustomObjects/Assets/DONT%20TOUCH/Scripts/BlockComponents/SchematicBlock.cs)
Diğer tüm nesnelerin türediği soyut (abstract) temel sınıftır. Unity'nin `MonoBehaviour` sınıfından miras alır.
* **`Compile(SchematicBlockData block)`**: Nesnenin yerel pozisyon, yönelim (Euler açıları), boyut (Scale) ve `Static` olup olmadığı gibi temel nitelikleri JSON formatına hazırlar.
* **`Decompile(...)`**: Kaydedilmiş bir JSON dosyasını okuyarak Unity sahnesinde hiyerarşiyi otomatik olarak geri inşa eder.

---

## 📖 Bileşen Detayları ve Kullanım Kılavuzu

Unity sahnenizde tasarladığınız nesnelere atanan ve onların oyundaki işlevlerini belirleyen ana bileşenlerin detayları:

---

### 1. 🟦 PrimitiveComponent (İlkel Şekil)
Unity temel geometrik şekillerini (Küp, Küre, Silindir, Kapsül, Düzlem) oyuna aktarır.
* **`Color`**: Objenin rengini belirler. **HDR (1.0 üzeri) parlak renkleri destekler.** HDR renkler oyunda neon/parlak görünüm sağlar.
* **`Collidable`**: İşaretliyse objenin içinden geçilemez, fiziksel bir engeli olur. İşaretli değilse oyuncular içinden geçebilir.
* **`Visible`**: İşaretliyse obje görünür olur. İşaretli değilse görünmez bir duvar (invisible wall) görevi görür.

---

### 2. 💡 LightComponent (Işık)
Odayı aydınlatmak için Point (Nokta) veya Spot ışıkları oluşturur.
* **`LightType`**: `Point` (her yöne ampul gibi aydınlatma) veya `Spot` (el feneri gibi konik aydınlatma).
* **`Color`**: Işığın rengini belirler. **HDR renkleri tam olarak destekler.** Böylece ışık parlaklığını sınırların (0-255) çok üzerine çıkarabilirsiniz.
* **`Intensity`**: Işığın gücü/şiddeti.
* **`Range`**: Işığın aydınlatma yarıçapı/mesafesi.
* **`ShadowType`**: Gölgelendirmenin `Soft` (Yumuşak), `Hard` (Sert) veya `None` (Gölgesiz) olup olmayacağını belirler.

---

### 3. 📝 TextComponent (3D Havada Yazı)
Oyun dünyasında havada asılı duran 3D yazılar (`TextToy`) oluşturur.
* **Nasıl Çalışır:** Unity Inspector'da değiştirdiğiniz **Renk (Color)**, **Yazı Boyutu (FontSize)** ve **Hizalama (Alignment)** özellikleri derleyici tarafından otomatik olarak standart HTML etiketlerine (Örn: `<color=#FF0000FF>`, `<size=30>`, `<align=center>`) sarılarak kaydedilir. Sunucu bu etiketleri doğrudan SCP:SL istemcisine gönderir ve oyun içinde mükemmel şekilde hizalanmış, renkli 3D yazılar oluşur.
* **Geri Yükleme (Decompile):** Şematik geri yüklendiğinde bu etiketler kod tarafından otomatik olarak çözümlenerek Unity Inspector'daki ilgili ayarlara geri atanır.

---

### 4. 🖱️ InteractableComponent (Etkileşim Colliderı)
Oyuncuların **E** tuşuna basarak (anında veya basılı tutarak) etkileşime girebileceği görünmez alanlar oluşturur.
* **`Shape`**: Etkileşim alanının şekli (`Box` veya `Sphere`).
* **`InteractionDuration`**: Etkileşim süresi (saniye). `0.0` yapılırsa E'ye basıldığı an çalışır. `3.0` yapılırsa oyuncunun E tuşuna 3 saniye basılı tutması gerekir.
* **`IsLocked`**: True ise etkileşim kilitlenir.
* **`TargetObject`**: Tetiklenecek animasyonu içeren **GameObject** (Cube veya parent nesne). Bu nesnenin kendisinde veya alt/üst objelerinde bir **Animator** olmalıdır.
* **`AnimationStateName`**: E tuşuna basıldığında Animator üzerinde oynatılacak animasyon durumunun (State) tam adı (Örn: `open`).
* **`AnimationStateName2`**: (Opsiyonel) Eğer bu alana da bir durum yazılırsa (Örn: `close`), E tuşuna her ardışık basıldığında iki durum arasında geçiş (toggle) yapılır.

> [!TIP]
> Kapı ve diğer nesnelerin etkileşimli animasyon kurulumu için adım adım [Animasyon Kurulum Rehberi (ANIMATION_TUTORIAL_TR.md)](file:///home/flaouve/Projects/scp%20sl/Map/Fla-SL-CustomObjects/Docs/ANIMATION_TUTORIAL_TR.md) sayfasını inceleyin.

---

### 5. 🎒 PickupComponent (Yerde Duran Eşya)
Oyun dünyasına yerleştirilen ve oyuncuların yerden alabildiği eşyaları, kartları veya silahları tanımlar.
* **`ItemType`**: Doğacak eşyanın türü (Örn: `KeycardO5`, `GunE11SR`).
* **`Locked` (Buton Modu):** Eğer bu seçenek işaretliyse, eşya yerden **alınamaz**. Bunun yerine oyuncu E tuşuna bastığında bu eşya bir etkileşim butonu görevi görür ve sunucu event'lerini tetikler.
* **`NumberOfUses` (Kullanım Sınırı):** Yerdeki eşyanın kaç kez alınabileceğini belirler.
  * `1`: Eşya bir kez alındığında yok olur (standart).
  * `5`: Eşya 5 kez alınabilir (her alındığında yerine yenisi doğar).
  * `-1`: **Sonsuz/Sınırsız** doğma noktası yapar (aldıkça yerine yenisi gelir).
* **`AttachmentsCode` (Silah Eklenti Kodu):** Silahların hangi dürbün, namlu, şarjör eklentileriyle doğacağını belirler.

#### 🔫 AttachmentsCode (Eklenti Kodu) Nasıl Alınır?
1. SCP:SL oyununa (yerel veya yetkili olduğunuz bir sunucuya) girin.
2. Envanterinizden eklentilerini özelleştirmek istediğiniz silahı elinize alın.
3. Silah menüsünü (Tab tuşu) açarak dilediğiniz dürbün, namlu, dipçik vb. eklentileri takın.
4. RA (Remote Admin) konsolunu (Konsol tuşu `~` veya `é`) açın ve şu komutu yazın:
   `forceatt`
5. Konsol size sayısal bir kod verecektir (Örn: `12456` veya benzeri uzun bir sayı).
6. Bu kodu kopyalayın ve Unity Inspector'daki **`Attachments Code`** alanına yapıştırın. Oyuncular bu silahı yerden aldıklarında taktığınız eklentilerle birlikte gelecektir.

---

### 6. 📍 WaypointComponent (Navigasyon Noktası)
Yapay Zekaların (Botların/Dummylerin) haritada yollarını bulabilmesi için navigasyon düğümleri oluşturur.
* **`Priority`**: Yapay zekanın bu yolu tercih etme önceliği (`0` ile `255` arası). `255` en yüksek önceliktir ve botlar yollarını seçerken önceliği yüksek olan waypoint'leri tercih ederler.

---

### 7. 🔧 WorkstationComponent (Çalışma Masası)
Oyuncuların silah eklentilerini oyun içinde değiştirebilmesini sağlayan standart MER çalışma masalarını spawn eder.
* **`IsInteractable`**: Masanın kullanılıp kullanılamayacağını belirler.

---

## ⚙️ Şablon Derleme Adımları

1. Unity projenizi açın.
2. Sahnenizde hiyerarşide sağ tıklayarak **🛠️ MER Blocks** menüsünden nesneler oluşturup haritanızı tasarlayın.
3. Tasarladığınız tüm objelerin tek bir üst nesnenin (kök) altında olmasına dikkat edin. Bu kök nesnede `Schematic` bileşeni yer almalıdır.
4. Kök nesneyi seçip Inspector panelinden **Compile Schematic** butonuna tıklayın.
5. Derlenen çıktıları (JSON ve varsa AssetBundle dosyalarını) dışa aktarma klasörünüzde (varsayılan olarak Masaüstünde) bulabilirsiniz.
6. Bu dosyaları oyun sunucunuzda aşağıdaki klasöre kopyalayın:
   `LabAPI/configs/ProjectMER/Schematics/[ŞablonAdınız]/`
