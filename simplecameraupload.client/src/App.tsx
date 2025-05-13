import React, { useRef, useState } from 'react';
import './App.css';

function App() {
    const fileInputRef = useRef<HTMLInputElement | null>(null);
    const [preview, setPreview] = useState<string | null>(null);
    const [uploadStatus, setUploadStatus] = useState<string>('');

    const handleCapture = (event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (file) {
            setPreview(URL.createObjectURL(file));
            uploadImage(file);
        }
    };

    const uploadImage = async (file: File) => {
        setUploadStatus('Uploading...');
        const formData = new FormData();
        formData.append('image', file);

        try {
            const response = await fetch('/api/camera/upload', {
                method: 'POST',
                body: formData,
            });

            if (response.ok) {
                setUploadStatus('Upload successful!');
            } else {
                setUploadStatus('Upload failed.');
            }
        } catch (error) {
            setUploadStatus(`Upload error: ${error}.`);
        }
    };

    return (
        <div style={{ padding: 24 }}>
            <h2>Take a Picture and Upload</h2>
            <input
                ref={fileInputRef}
                type="file"
                accept="image/*"
                capture="environment"
                style={{ display: 'block', marginBottom: 16 }}
                onChange={handleCapture}
            />
            {preview && (
                <div>
                    <img src={preview} alt="Preview" style={{ maxWidth: 300, marginBottom: 16 }} />
                </div>
            )}
            <div>{uploadStatus}</div>
        </div>
    );
};

export default App;