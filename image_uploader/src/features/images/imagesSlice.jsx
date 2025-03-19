import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { fetchImages, uploadImage } from './imagesApi';

// Fetch images from GridFS
export const getImages = createAsyncThunk('images/getImages', async () => {
  return await fetchImages();
});

// Upload an image
export const addImage = createAsyncThunk('images/addImage', async (formData) => {
  return await uploadImage(formData);
});

const imagesSlice = createSlice({
  name: 'images',
  initialState: {
    images: [],
    status: 'idle',
    error: null,
  },
  extraReducers: (builder) => {
    builder
      .addCase(getImages.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(getImages.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.images = action.payload; // Store image metadata
      })
      .addCase(getImages.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.error.message;
      })
      .addCase(addImage.fulfilled, (state, action) => {
        state.images.push(action.payload); // Add new image to the list
      });
  },
});

export default imagesSlice.reducer;
