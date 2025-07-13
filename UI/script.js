document.addEventListener('DOMContentLoaded', () => {
    // ドラッグ＆ドロップなど、基本的な機能は先に設定
    const imageUploader = document.getElementById('imageUploader');
    const canvas = document.getElementById('canvas');
    const leftStock = document.getElementById('left-stock');
    const rightStock = document.getElementById('right-stock');

    let draggedItem = null;
    let offsetX = 0;
    let offsetY = 0;
    let isDraggingFromStock = false;

    // 1. 画像アップロード機能
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

    // 2. ストックエリアの画像にイベントを設定
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

    // 3. キャンバス内の画像にイベントを設定
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

    // 4. マウス移動時の処理
    document.addEventListener('mousemove', (e) => {
        if (!draggedItem) return;
        e.preventDefault();

        if (isDraggingFromStock && !document.getElementById('drag-preview')) {
            const preview = draggedItem.cloneNode();
            preview.id = 'drag-preview';
            preview.style.position = 'absolute';
            preview.style.pointerEvents = 'none';
            preview.style.opacity = '0.7';
            preview.style.maxWidth = '300px';
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

    // 5. マウスボタンを離した時の処理
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
            const newImg = draggedItem.cloneNode();
            newImg.className = 'draggable-image';
            newImg.style.width = '';
            newImg.style.height = '';
            newImg.style.left = `${e.clientX - canvasRect.left - offsetX}px`;
            newImg.style.top = `${e.clientY - canvasRect.top - offsetY}px`;

            addDragEventsToCanvasImage(newImg);
            canvas.appendChild(newImg);
        }

        draggedItem.style.cursor = 'grab';
        draggedItem = null;
        isDraggingFromStock = false;
    });
});

window.addEventListener('load', () => {
    const canvas = document.getElementById('canvas');
    const movingImage = document.createElement('img');

    movingImage.src = 'images/box.png';
    
    // ★アニメーション用のCSSクラスを割り当てるだけに変更
    movingImage.className = 'animated-center-image';
    
    canvas.appendChild(movingImage);
});