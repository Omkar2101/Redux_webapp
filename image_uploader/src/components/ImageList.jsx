
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { getImages } from '../features/images/imagesSlice';
import { getImageUrl } from '../features/images/imagesApi';

const ImageList = () => {
  const dispatch = useDispatch();
  const { images, status, error } = useSelector((state) => state.images);

  useEffect(() => {
    dispatch(getImages());
  }, [dispatch]);

  if (status === 'loading') return <p className="text-center text-blue-500">Loading images...</p>;
  
  if (status === 'failed') {
    alert(" Failed to load images!");
    return <p className="text-red-500 text-center">Error: {error}</p>;
  }

  return (
    <div className="grid mt-4 grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
      {images.length === 0 ? (
        <p className="col-span-full text-center text-gray-500">No images uploaded yet.</p>
      ) : (
        images.map((image) => (
          <div key={image.id} className="border rounded-lg overflow-hidden shadow-md">
            <img src={getImageUrl(image.id)} alt="Uploaded" className="w-full h-40 object-cover" />
          </div>
        ))
      )}
    </div>
  );
};

export default ImageList;
