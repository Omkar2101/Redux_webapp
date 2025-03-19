import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { addImage } from '../features/images/imagesSlice';

const ImageUpload = () => {
  const [file, setFile] = useState(null);
  const dispatch = useDispatch();

  const handleFileChange = (e) => {
    setFile(e.target.files[0]);
  };

  const handleUpload = async () => {
    if (!file) {
      alert("Please select a file before uploading.");
      return;
    }

    const formData = new FormData();
    formData.append('file', file);

    try {
      await dispatch(addImage(formData)).unwrap();
      alert(" Image uploaded successfully!");
      setFile(null); 
    } catch (error) {
      alert(" Image upload failed. Please try again.");
      console.error("Upload Error:", error);
    }
  };

  return (
    <div className="p-4 border rounded-lg shadow-md">
      <input type="file" onChange={handleFileChange} className="mb-2" />
      <button onClick={handleUpload} className="bg-blue-500 text-white px-4 py-2 rounded">
        Upload
      </button>
    </div>
  );
};

export default ImageUpload;
