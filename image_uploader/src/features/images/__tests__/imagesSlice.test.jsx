import imagesReducer, { getImages, addImage } from '../imagesSlice';
import {  test, describe, expect } from 'vitest';

console.log('Test file loaded');
describe('imagesSlice', () => {
  const initialState = {
    images: [],
    status: 'idle',
    error: null,
  };

  test('should handle initial state', () => {
    expect(imagesReducer(undefined, { type: 'unknown' })).toEqual(initialState);
  });

  test('should handle getImages.pending', () => {
    const action = { type: getImages.pending.type };
    const state = imagesReducer(initialState, action);
    expect(state.status).toBe('loading');
  });

  test('should handle getImages.fulfilled', () => {
    const mockPayload = [{ id: 'img1' }, { id: 'img2' }];
    const action = { type: getImages.fulfilled.type, payload: mockPayload };
    const state = imagesReducer(initialState, action);
    expect(state.status).toBe('succeeded');
    expect(state.images).toEqual(mockPayload);
  });

  test('should handle getImages.rejected', () => {
    const action = {
      type: getImages.rejected.type,
      error: { message: 'Fetch failed' },
    };
    const state = imagesReducer(initialState, action);
    expect(state.status).toBe('failed');
    expect(state.error).toBe('Fetch failed');
  });

  test('should handle addImage.fulfilled', () => {
    const newImage = { id: 'newImage' };
    const action = { type: addImage.fulfilled.type, payload: newImage };
    const state = imagesReducer(initialState, action);
    expect(state.images).toContainEqual(newImage);
  });
});
