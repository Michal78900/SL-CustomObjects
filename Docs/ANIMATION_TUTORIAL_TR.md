# 🎬 MER Animasyon Kurulum ve Etkileşim Kılavuzu (Türkçe)

Bu kılavuz, Unity'de hazırladığınız nesnelerin (Örn: Kapılar, asansörler, hareketli platformlar) oyuncuların **E** tuşuna basmasıyla tetiklenecek şekilde nasıl kurulacağını adım adım anlatmaktadır.

---

## 🏗️ 1. Unity'de Animator ve Animasyon Hazırlığı

Animasyonlu nesnenizin (Örn: Kırmızı bir kapı) sorunsuz çalışması için Animator Controller ve Animasyon dosyalarının doğru yapılandırılması gerekir:

1. **Animator Ekleme:**
   * Hareket ettirmek istediğiniz objeyi (Örn: `Kapi_Mesh`) seçin.
   * Inspector panelinden **Add Component -> Animator** ekleyin.
2. **Animator Controller Oluşturma:**
   * Proje penceresinde (Project) boş bir yere sağ tıklayın: **Create -> Animator Controller** oluşturun ve adını `Kapi_Controller` koyun.
   * Animator component'indeki **Controller** alanına bu oluşturduğunuz `Kapi_Controller` dosyasını sürükleyip bırakın.
3. **Animasyon Klipleri Oluşturma:**
   * Objeniz seçiliyken **Animation** penceresini açın (Window -> Animation -> Animation).
   * **Create** butonuna tıklayarak ilk animasyonu oluşturun. Adını **`open`** yapın (Kapının açılma hareketi).
   * İkinci bir klip oluşturup adını **`close`** yapın (Kapının kapanma hareketi).
   * Animasyonlarınızı kaydedin ve nesnenizin transform hareketlerini kaydederek animasyonları tamamlayın.
4. **Durumların (States) Animator'a Eklenmesi:**
   * **Animator** penceresini açın.
   * Oluşturduğunuz `open` ve `close` animasyon kliplerinin bu pencerede birer kutucuk (State) olarak yer aldığını doğrulayın.
   * **Çok Önemli:** Durumların (State) isimleri tam olarak `open` ve `close` olmalıdır (küçük harflerle).

---

## 🖱️ 2. Interactable (Etkileşim Tetikleyici) Kurulumu

Oyuncunun E tuşuna basarak bu animasyonu tetikleyebilmesi için bir tetikleyici alan eklememiz gerekir.

> [!WARNING]
> **Kritik Hata Nedeni:** Unity'de bir obje üzerine hem `PrimitiveComponent` (Küp vb.) hem de `InteractableComponent` scriptlerini **aynı anda eklemeyin**. Bu işlem, sunucu şablonu yüklerken aynı obje ID'sinin çakışmasına ve sunucunun çökmesine (crash) yol açar.

### Doğru Hiyerarşi Yapısı:
```
── 🛠️ Kök Şablon Nesnesi (Schematic Component)
   ├── 🟦 Kapi_Mesh (PrimitiveComponent + Animator)  <-- Sadece görsel ve animasyon
   └── 🖱️ Kapi_Tetikleyici (InteractableComponent)   <-- Sadece etkileşim alanı
```

### Adım Adım Kurulum:
1. Hiyerarşide (Hierarchy) sağ tıklayın: **`🛠️ MER Blocks -> Interactable`** seçerek yeni bir etkileşim alanı oluşturun.
2. Oluşturulan tetikleyici nesnesini kapınızın önüne/yanına (oyuncunun E tuşuna basacağı alana) konumlandırın.
3. Tetikleyiciyi seçin ve Inspector'dan şu ayarları yapın:
   * **`Target Object`**: Animasyona sahip olan nesnenizi (`Kapi_Mesh` veya bu nesneyi içeren üst/alt objeyi) bu alana sürükleyip bırakın.
     * *Yeni eklediğimiz dinamik arama sistemi sayesinde, sürüklediğiniz nesnenin kendisinde, alt objelerinde veya ebeveyn objelerinde Animator varsa sunucu bunu otomatik olarak bulacaktır.*
   * **`Animation State Name`**: İlk etkileşimde oynatılacak durum adını yazın: **`open`**
   * **`Animation State Name 2`**: (Opsiyonel) İkinci etkileşimde (kapatmak için) oynatılacak durum adını yazın: **`close`**
   * **`Interaction Duration`**: Anında tetiklenme için `0` yapın. Basılı tutma barı için saniye cinsinden değer girin (Örn: `3.0`).

---

## 📦 3. Derleme (Compile) ve Sunucuya Aktarma

1. Şablonunuzun kök nesnesini seçin ve **`Compile Schematic`** butonuna tıklayın.
2. Derleyici, animasyonlarınızı otomatik olarak sunucunun okuyabileceği bir **AssetBundle** dosyasına (Örn: şablon isminize göre uzantısız dosya) paketleyecektir.
3. Masaüstünüze çıkan klasörü (Örn: `test3/`) sunucunuzun şu dizinine yükleyin:
   `LabAPI/configs/ProjectMER/Schematics/test3/`
4. Klasör içeriğinin şu şekilde göründüğünden emin olun:
   * `test3.json`
   * `Cube` (Uzantısız AssetBundle dosyası)
