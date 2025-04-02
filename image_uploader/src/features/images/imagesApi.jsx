import axios from 'axios';

const API_URL = 'http://localhost:8080/api/files'; // Your backend API

// Fetch all uploaded images
export const fetchImages = async () => {
  try {
    const response = await axios.get(API_URL);
    return response.data;
  } catch (error) {
    console.error("Error fetching images:", error);
    throw error;
  }
};

// Upload an image to GridFS
export const uploadImage = async (formData) => {
  try {
    const response = await axios.post(`${API_URL}/upload`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  } catch (error) {
    console.error("Error uploading image:", error);
    throw error;
  }
};

// Get the file URL for displaying images
export const getImageUrl = (fileId) => {
  return `${API_URL}/${fileId}`; // Assuming your backend serves images at this route
};
