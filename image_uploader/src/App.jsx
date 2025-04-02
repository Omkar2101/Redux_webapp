import React from 'react';
import ImageUpload from './components/ImageUpload';
import ImageList from './components/ImageList';

function App() {
  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-4">Image Uploader</h1>
      <h2>Click below to upload</h2>
      <ImageUpload />
      <ImageList />
    </div>
  );
}

export default App;
