document.addEventListener('DOMContentLoaded', () => {
    const imageUploader = document.getElementById('imageUploader');
    const canvas = document.getElementById('canvas');
    const leftStock = document.getElementById('left-stock');
    const rightStock = document.getElementById('right-stock');

    let draggedItem = null;
    let offsetX = 0;
    let offsetY = 0;
    let isDraggingFromStock = false;

    /**
     * ★★ 1. 初期画像をストックに読み込む関数 ★★
     * ページを開いたときに、指定した画像を左右のストックに配置します。
     * 画像のパスはここで指定してください。
     */
    function loadInitialImages() {
        // 表示したい画像の情報を設定
        const initialImages = [
            { src: 'images/Blue_Souleater.png', targetArea: leftStock },
            { src: 'images/Blue_Bringer.png', targetArea: leftStock },
            { src: 'images/Blue_tower.png', targetArea: leftStock },
            { src: 'images/Red_Usurper.png', targetArea: rightStock },
            { src: 'images/Red_tower.png', targetArea: rightStock },
        ];

        initialImages.forEach(data => {
            const img = document.createElement('img');
            img.src = data.src;
            img.className = 'stock-image';
            // 画像が見つからない場合のエラー処理
            img.onerror = () => console.error(`画像の読み込みに失敗しました: ${data.src}`);
            
            addDragEventsToStockImage(img); // 画像にイベントを設定
            data.targetArea.appendChild(img); // ストックエリアに画像を追加
        });
    }


    // ファイルアップロード機能（必要なければこのセクションごと削除してもOKです）
    imageUploader.addEventListener('change', (event) => {
        const files = event.target.files;
        if (!files.length) return;

        const selectedStockValue = document.querySelector('input[name="player-stock"]:checked').value;
        const targetStockArea = (selectedStockValue === 'p1') ? leftStock : rightStock;

        for (const file of files) {
            const reader = new FileReader();
            reader.onload = (e) => {
                const img = document.createElement('img');
                img.src = e.target.result;
                img.className = 'stock-image';
                addDragEventsToStockImage(img);
                targetStockArea.appendChild(img);
            };
            reader.readAsDataURL(file);
        }
    });

    // ストックエリアの画像にイベントを設定する関数
    function addDragEventsToStockImage(img) {
        img.addEventListener('mousedown', (e) => {
            e.preventDefault();
            draggedItem = img;
            isDraggingFromStock = true;
            offsetX = e.offsetX;
            offsetY = e.offsetY;
            draggedItem.style.cursor = 'grabbing';
        });

        img.addEventListener('dblclick', () => {
            img.remove();
        });
    }

    // キャンバス内の画像にイベントを設定する関数
    function addDragEventsToCanvasImage(img) {
        img.addEventListener('mousedown', (e) => {
            e.preventDefault();
            draggedItem = img;
            isDraggingFromStock = false;
            offsetX = e.clientX - draggedItem.getBoundingClientRect().left;
            offsetY = e.clientY - draggedItem.getBoundingClientRect().top;
            draggedItem.style.cursor = 'grabbing';
        });

        img.addEventListener('dblclick', () => {
            img.remove();
        });
    }

    // マウス移動時の処理
    document.addEventListener('mousemove', (e) => {
        if (!draggedItem) return;
        e.preventDefault();

        if (isDraggingFromStock && !document.getElementById('drag-preview')) {
            const preview = draggedItem.cloneNode();
            preview.id = 'drag-preview';
            preview.style.position = 'absolute';
            preview.style.pointerEvents = 'none';
            preview.style.opacity = '0.7';
            preview.style.maxWidth = '250px';
            document.body.appendChild(preview);
        }

        const preview = document.getElementById('drag-preview');

        if (isDraggingFromStock && preview) {
            preview.style.left = `${e.clientX - offsetX}px`;
            preview.style.top = `${e.clientY - offsetY}px`;
        } else if (!isDraggingFromStock) {
            const canvasRect = canvas.getBoundingClientRect();
            const x = e.clientX - canvasRect.left - offsetX;
            const y = e.clientY - canvasRect.top - offsetY;
            draggedItem.style.left = `${x}px`;
            draggedItem.style.top = `${y}px`;
        }
    });

    // マウスボタンを離した時の処理
    document.addEventListener('mouseup', (e) => {
        if (!draggedItem) return;

        const preview = document.getElementById('drag-preview');
        if (preview) {
             document.body.removeChild(preview);
        }

        const canvasRect = canvas.getBoundingClientRect();
        const isOverCanvas = e.clientX >= canvasRect.left && e.clientX <= canvasRect.right &&
                             e.clientY >= canvasRect.top && e.clientY <= canvasRect.bottom;

        if (isDraggingFromStock && isOverCanvas) {
            // 新しい画像を作成してキャンバスに追加
            const newImg = draggedItem.cloneNode(true); // クローンを作成
            newImg.className = 'draggable-image';
            newImg.style.width = '';
            newImg.style.height = '';
            newImg.style.left = `${e.clientX - canvasRect.left - offsetX}px`;
            newImg.style.top = `${e.clientY - canvasRect.top - offsetY}px`;

            addDragEventsToCanvasImage(newImg);
            canvas.appendChild(newImg);

            // ★★ 2. ストックにあった元の画像を削除 ★★
            draggedItem.remove();

        }

        if (draggedItem) { // draggedItemが削除されていない場合（移動しなかった場合）
            draggedItem.style.cursor = 'grab';
        }
        
        draggedItem = null;
        isDraggingFromStock = false;
    });

    // ページ読み込み完了時に初期画像を配置する
    loadInitialImages();
});