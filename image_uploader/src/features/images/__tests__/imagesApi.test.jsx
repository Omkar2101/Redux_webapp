import axios from 'axios';
import { vi, test, describe, expect } from 'vitest';
import { fetchImages, uploadImage, getImageUrl } from '../imagesApi';

vi.mock('axios');

describe('imagesApi', () => {
  test('fetchImages returns data on success', async () => {
    const mockData = [{ id: '1' }, { id: '2' }];
    axios.get.mockResolvedValue({ data: mockData });

    const result = await fetchImages();
    expect(result).toEqual(mockData);
    expect(axios.get).toHaveBeenCalledWith('http://localhost:5298/api/files');
  });

  test('uploadImage posts form data and returns response', async () => {
    const formData = new FormData();
    const mockResponse = { id: '123' };

    axios.post.mockResolvedValue({ data: mockResponse });

    const result = await uploadImage(formData);
    expect(result).toEqual(mockResponse);
    expect(axios.post).toHaveBeenCalled();
  });

  test('getImageUrl returns correct URL', () => {
    const fileId = 'abc123';
    expect(getImageUrl(fileId)).toBe(`http://localhost:5298/api/files/${fileId}`);
  });
});
